using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.AST.Utils;
using Ripple.Lexing;
using Ripple.Utils;

namespace Ripple.Validation
{
    class BasicTypeCheckStep : IStatementVisitor
    {
        private readonly ASTInfo m_ASTInfo;
        private readonly LocalVariableStack m_LocalVariables = new LocalVariableStack();
        public readonly List<ValidationError> Errors = new List<ValidationError>();

        private int m_LoopCount = 0;
        private FuncDecl m_CurrentFunc = null;

        public BasicTypeCheckStep(ASTInfo astInfo)
        {
            m_ASTInfo = astInfo;
            m_ASTInfo.AST.Accept(this);
        }

        public void VisitBlockStmt(BlockStmt blockStmt)
        {
            m_LocalVariables.PushScope();
            foreach (var stmt in blockStmt.Statements)
                stmt.Accept(this);
            m_LocalVariables.PopScope();
        }

        public void VisitBreakStmt(BreakStmt breakStmt)
        {
            if (m_LoopCount <= 0)
                AddError("Break statement must be inside a loop.", breakStmt.BreakToken);
        }

        public void VisitContinueStmt(ContinueStmt continueStmt)
        {
            if (m_LoopCount <= 0)
                AddError("Continue statement must be inside a loop.", continueStmt.ContinueToken);
        }

        public void VisitExprStmt(ExprStmt exprStmt)
        {
            CheckExpression(exprStmt.Expr, new Option<TypeInfo>());
        }

        public void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl) { }

        public void VisitFileStmt(FileStmt fileStmt)
        {
            foreach (Statement statement in fileStmt.Statements)
                statement.Accept(this);
        }

        public void VisitForStmt(ForStmt forStmt)
        {
            m_LocalVariables.PushScope();
            forStmt.Init.Match(init => init.Accept(this));
            forStmt.Condition.Match(cond =>
            {
                VerifyReturnBool(cond, forStmt.ForTok);
            });
            forStmt.Iter.Match(iter => CheckExpression(iter, new Option<TypeInfo>()));

            m_LoopCount++;
            forStmt.Body.Accept(this);
            m_LoopCount--;

            m_LocalVariables.PopScope();
        }

        private void VerifyReturnBool(Expression cond, Token tokenForError)
        {
            CheckExpression(cond, RipplePrimitives.Bool).Match(type =>
            {
                if (!type.Equals(RipplePrimitives.Bool))
                    AddError("Condition expression must evaluate to a bool.", tokenForError);
            });
        }

        public void VisitFuncDecl(FuncDecl funcDecl)
        {
            m_CurrentFunc = funcDecl;
            m_LocalVariables.PushScope();

            foreach ((TypeName type, Token name) in funcDecl.Param.ParamList)
            {
                m_LocalVariables.AddVariable(name, TypeInfo.FromASTType(type), funcDecl.UnsafeToken.HasValue, (p) =>
                {
                    AddError("Parameter: " + p.Text + " Is already defined.", p);
                });
            }

            funcDecl.Body.Accept(this);
            m_LocalVariables.PopScope();
            m_CurrentFunc = null;
        }

        public void VisitGenericParameters(GenericParameters genericParameters) { }

        public void VisitIfStmt(IfStmt ifStmt)
        {
            VerifyReturnBool(ifStmt.Expr, ifStmt.IfTok);

            m_LocalVariables.PushScope();
            ifStmt.Body.Accept(this);
            m_LocalVariables.PopScope();

            m_LocalVariables.PushScope();
            ifStmt.ElseBody.Match(body => body.Accept(this));
            m_LocalVariables.PopScope();
        }

        public void VisitParameters(Parameters parameters) { }

        public void VisitProgramStmt(ProgramStmt programStmt)
        {
            foreach (FileStmt file in programStmt.Files)
                file.Accept(this);
        }

        public void VisitReturnStmt(ReturnStmt returnStmt)
        {
            TypeInfo returnTypeInfo = TypeInfo.FromASTType(m_CurrentFunc.ReturnType);
            if (returnTypeInfo.Equals(RipplePrimitives.Void))
            {
                returnStmt.Expr.Match(ok =>
                {
                    AddError("Cannot return a value from a void function.", returnStmt.ReturnTok);
                });
            }
            else
            {
                returnStmt.Expr.Match(expr =>
                {
                    CheckExpression(expr, returnTypeInfo).Match(returned =>
                    {
                        if (!returned.Equals(returnTypeInfo))
                        {
                            AddError("Cannot return value of type: " +
                                returned +
                                " from function with return type: " +
                                returnTypeInfo, returnStmt.ReturnTok);
                        }
                    });
                },
                () =>
                {
                    AddError("Return statement must return a value.", returnStmt.ReturnTok);
                });
            }
        }

        public void VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            foreach (Statement statement in unsafeBlock.Statements)
                statement.Accept(this);
        }

        public void VisitVarDecl(VarDecl varDecl)
        {
            TypeInfo varType = TypeInfo.FromASTType(varDecl.Type);
            TypeInfo varTypeChecker = varType.ChangeMutable(false);

            CheckExpression(varDecl.Expr, varType).Match(ok =>
            {
                var binaryOperators = m_ASTInfo.OperatorLibrary.BinaryOperators;
                if (!binaryOperators.Contains(TokenType.Equal, (varType.ChangeMutable(true), ok.Type)))
                {
                    AddError("Variable of type: " + varType +
                             " cannot be assigned to a expression evaluating to type: " +
                             ok, varDecl.VarNames[0]);
                }
                else
                {
                    AddVeriables(varDecl.VarNames, ok.Type, varDecl.UnsafeToken.HasValue);
                }
            });
        }

        private void AddVeriables(List<Token> names, TypeInfo type, bool isUnsafe)
        {
            if (m_CurrentFunc != null)
            {
                m_LocalVariables.AddVariables(names, type, isUnsafe, v =>
                {
                    AddError("Variable: " + v.Text + " Is already defined.", v);
                });
            }
        }

        public void VisitWhereClause(WhereClause whereClause)
        {
            throw new NotImplementedException();
        }

        public void VisitWhileStmt(WhileStmt whileStmt)
        {
            VerifyReturnBool(whileStmt.Condition, whileStmt.WhileToken);
            m_LoopCount++;
            m_LocalVariables.PushScope();
            whileStmt.Body.Accept(this);
            m_LocalVariables.PopScope();
            m_LoopCount--;
        }

        private Option<ValueInfo> CheckExpression(Expression expression, Option<TypeInfo> expected)
        {
            var result = ValueInfo.FromExpression(m_ASTInfo, m_LocalVariables, expression, expected);
            Option<ValueInfo> info = new Option<ValueInfo>();
            result.Match(ok => { info = ok; }, fail => AddError(fail.Message, fail.Token));
            return info;
        }

        private void AddError(string message, Token token)
        {
            Errors.Add(new ValidationError(message, token));
        }
    }
}
