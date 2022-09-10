using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class TypeCheckerVisitor : IStatementVisitor, IExpressionVisitor<string>
    {
        private readonly TypeCheckerHelper m_Helper;
        private readonly List<ValidationError> m_Errors = new List<ValidationError>();
        private readonly LocalVariableStack m_LocalVarStack = new LocalVariableStack();

        private string m_CurrentFunctionReturnType = null;
        bool m_IsInFunction = false;

        bool m_IsInForOrIf = false;
        bool m_DoesFunctionHaveReturn = false;

        public TypeCheckerVisitor(TypeCheckerHelper helper)
        {
            m_Helper = helper;
        }

        public List<ValidationError> TypeCheck(FileStmt fileStmt)
        {
            m_Errors.Clear();
            m_LocalVarStack.Clear();
            fileStmt.Accept(this);
            return m_Errors;
        }

        public void VisitBlockStmt(BlockStmt blockStmt)
        {
            m_LocalVarStack.PushScope();
            foreach (Statement statement in blockStmt.Statements)
                statement.Accept(this);
            m_LocalVarStack.PopScope();
        }

        public void VisitExprStmt(ExprStmt exprStmt)
        {
            TryTypeCheckExpression(exprStmt.Expr, out _);
        }

        public void VisitFileStmt(FileStmt fileStmt)
        {
            foreach (Statement declaration in fileStmt.Statements)
                declaration.Accept(this);
        }

        public void VisitForStmt(ForStmt forStmt)
        {
            bool wasInForOrIf = m_IsInForOrIf;
            m_IsInForOrIf = true;

            m_LocalVarStack.PushScope();
            forStmt.Init.Accept(this);

            if(TryTypeCheckExpression(forStmt.Condition, out string type) &&
                type != RipplePrimitiveNames.Bool)
            {
                string message = "For conditional expression must be of type bool.";
                m_Errors.Add(new ValidationError(message, forStmt.ForTok));
            }

            TryTypeCheckExpression(forStmt.Iter, out _);

            forStmt.Body.Accept(this);
            m_LocalVarStack.PopScope();

            if (!wasInForOrIf)
                m_IsInForOrIf = false;
        }

        public void VisitFuncDecl(FuncDecl funcDecl)
        {
            string returnTypeName = funcDecl.ReturnType.Text;
            if(m_Helper.ContainsType(returnTypeName))
            {
                m_CurrentFunctionReturnType = returnTypeName;
            }
            else
            {
                string message = "Return type" + returnTypeName + " does not exist.";
                m_Errors.Add(new ValidationError(message, funcDecl.ReturnType));
            }
            m_IsInFunction = true;
            
            m_LocalVarStack.PushScope();
            funcDecl.Param.Accept(this);
            funcDecl.Body.Accept(this);
            m_LocalVarStack.PopScope();

            if(returnTypeName != RipplePrimitiveNames.Void && !m_DoesFunctionHaveReturn)
            {
                string message = "Non-void function must have a return statement.";
                m_Errors.Add(new ValidationError(message, funcDecl.Arrow));
            }

            m_IsInFunction = false;
            m_CurrentFunctionReturnType = null;
            m_DoesFunctionHaveReturn = false;
        }

        public void VisitIfStmt(IfStmt ifStmt)
        {
            bool wasInForOrIf = m_IsInForOrIf;
            m_IsInForOrIf = true;

            if(TryTypeCheckExpression(ifStmt.Expr, out string type) &&
               type != RipplePrimitiveNames.Bool)
            {
                string message = "Expression in an if statement must be of type bool.";
                m_Errors.Add(new ValidationError(message, ifStmt.IfTok));
            }
            ifStmt.Body.Accept(this);

            if (!wasInForOrIf)
                m_IsInForOrIf = false;
        }

        public void VisitParameters(Parameters parameters)
        {
            foreach ((var type, var name) in parameters.ParamList)
                AddVariable(type, name);
        }

        public void VisitReturnStmt(ReturnStmt returnStmt)
        {
            if(!m_IsInFunction)
            {
                string message = "Return statement must be inside of a function body.";
                m_Errors.Add(new ValidationError(message, returnStmt.ReturnTok));
                return;
            }

            if(returnStmt.Expr == null)
            {
                if(m_CurrentFunctionReturnType != RipplePrimitiveNames.Void)
                {
                    string message = "Return statement must return the same type as the function.";
                    m_Errors.Add(new ValidationError(message, returnStmt.ReturnTok));
                }
                return;
            }

            if(m_CurrentFunctionReturnType != null)
            {
                if(TryTypeCheckExpression(returnStmt.Expr, out string type)&& 
                   type != m_CurrentFunctionReturnType)
                {
                    string message = "Return statement must return the same type as the function.";
                    m_Errors.Add(new ValidationError(message, returnStmt.ReturnTok));
                    return;
                }
            }


            if (!m_IsInForOrIf)
                m_DoesFunctionHaveReturn = true;
        }

        public void VisitVarDecl(VarDecl varDecl)
        {
            AddVariable(varDecl);
        }

        public string VisitBinary(Binary binary)
        {
            string leftType = binary.Right.Accept(this);
            string rightType = binary.Right.Accept(this);
            List<string> args = new List<string>() { leftType, rightType };

            if(m_Helper.TryGetOperator(binary.Op.Type, args, out OperatorData.Binary operatorData))
            {
                return operatorData.ReturnType;
            }
            else
            {
                string message = "Binary operator: " + binary.Op.Text + " does not have an overload for types: "
                    + leftType + ", " + rightType + ".";

                throw new TypeCheckExeption(message, binary.Op);
            }
        }

        public string VisitCall(Call call)
        {
            List<string> argTypes = new List<string>();
            foreach (Expression expression in call.Args)
                argTypes.Add(expression.Accept(this));

            string funcName = call.Identifier.Text;
            if(m_Helper.TryGetFunction(funcName, argTypes, out FunctionData functionData))
            {
                return functionData.ReturnType.Text;
            }
            else
            {
                string message = m_Helper.ContainsFunctionWithName(funcName) ?
                    "Funtion: " + funcName + "does not have an overload with the given argument types defined." :
                    "Function: " + funcName + " is not defined.";

                throw new TypeCheckExeption(message, call.Identifier);
            }
        }

        public string VisitGrouping(Grouping grouping)
        {
            return grouping.Expr.Accept(this);
        }

        public string VisitIdentifier(Identifier identifier)
        {
            string variableName = identifier.Name.Text;

            bool variableExists =
                m_LocalVarStack.TryGetVariable(variableName, out VariableData variable) ||
                m_Helper.TryGetVariable(variableName, out variable);

            if(variableExists)
            {
                return variable.Type.Text;
            }
            else
            {
                string message = "Variable: " + variableName + " is not defined.";
                throw new TypeCheckExeption(message, identifier.Name);
            }
        }

        public string VisitLiteral(Literal literal)
        {
            return literal.Val.Type switch
            {
                TokenType.IntagerLiteral => RipplePrimitiveNames.Int32,
                TokenType.True or TokenType.False => RipplePrimitiveNames.Bool,
                TokenType.FloatLiteral => RipplePrimitiveNames.Float32,
                _ => throw new ArgumentException("Literal is of a unknown type."),
            };
        }

        public string VisitUnary(Unary unary)
        {
            string operandType = unary.Expr.Accept(this);
            if (m_Helper.TryGetOperator(unary.Op.Type, new List<string> { operandType }, out OperatorData.Unary data))
            {
                return data.ReturnType;
            }
            else
            {
                string message = "Unary operator: " + unary.Op.Text + " does not have an overload for type: "
                    + operandType + ".";

                throw new TypeCheckExeption(message, unary.Op);
            }
        }

        private bool TryTypeCheckExpression(Expression expression, out string type)
        {
            type = null;

            try
            {
                type = expression.Accept(this);
                return true;
            }
            catch(TypeCheckExeption e)
            {
                m_Errors.Add(new ValidationError(e.Message, e.ErrorToken));
                return false;
            }
        }

        private void AddVariable(VarDecl varDecl)
        {
            foreach (Token name in varDecl.VarNames)
                AddVariable(varDecl.TypeName, name);
        }

        private void AddVariable(Token type, Token name)
        {
            if(!m_Helper.ContainsType(type.Text))
            {
                string message = "Expected a type name.";
                m_Errors.Add(new ValidationError(message, type));
                return;
            }

            if(m_Helper.ContainsType(name.Text))
            {
                string message = "Local variable: " + name.Text + " name cannot be a type name.";
                m_Errors.Add(new ValidationError(message, name));
                return;
            }

            if(m_Helper.ContainsFunctionWithName(name.Text))
            {
                string message = "Local variable: " + name.Text + " name cannot be a function name.";
                m_Errors.Add(new ValidationError(message, name));
                return;
            }

            if(m_Helper.ContainsVariable(name.Text))
            {
                string message = "Local variable: " + name.Text + " name cannot be a global variable name";
                m_Errors.Add(new ValidationError(message, name));
                return;
            }

            VariableData data = new VariableData(name, type);
            if (!m_LocalVarStack.TryAddVariable(data))
            {
                string message = "Local variable: " + name.Text + " is already defined.";
                m_Errors.Add(new ValidationError(message, name));
                return;
            }
        }
    }
}
