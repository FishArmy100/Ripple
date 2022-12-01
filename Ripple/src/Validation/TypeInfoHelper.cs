using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Info;
using Ripple.AST;
using Ripple.Utils;
using Ripple.Lexing;
using Ripple.AST.Utils;

namespace Ripple.Validation
{
    static class TypeInfoHelper
    {
        public struct Error
        {
            public Token Token;
            public string Message;

            public Error(Token token, string message)
            {
                Token = token;
                Message = message;
            }
        }

        public static Result<TypeInfo, Error> GetTypeOfExpression(ASTInfo ast, LocalVariableStack localVariables, Expression expr)
        {
            TypeOfExpressionVisitor visitor = new TypeOfExpressionVisitor(ast, localVariables);
            try
            {
                return new Result<TypeInfo, Error>.Ok(expr.Accept(visitor));
            }
            catch(TypeOfExpressionExeption e)
            {
                return new Result<TypeInfo, Error>.Fail(new Error(e.ErrorToken, e.Message));
            }
        }

        private class TypeOfExpressionVisitor : IExpressionVisitor<TypeInfo>
        {
            private OperatorLibrary m_OperatorLibrary => m_ASTInfo.OperatorLibrary;
            private readonly LocalVariableStack m_VariableStack;
            private readonly ASTInfo m_ASTInfo;

            public TypeOfExpressionVisitor(ASTInfo astInfo, LocalVariableStack variableStack)
            {
                m_VariableStack = variableStack;
                m_ASTInfo = astInfo;
            }

            public TypeInfo VisitBinary(Binary binary)
            {
                TypeInfo left = binary.Left.Accept(this);
                TypeInfo right = binary.Right.Accept(this);
                TokenType operatorType = binary.Op.Type;

                if (m_OperatorLibrary.BinaryOperators.TryGet(operatorType, (left, right), out OperatorInfo.Binary info))
                    return info.Returned;

                string message = "No binary operator: " + operatorType.ToString() + ", for types: " +
                    left.ToPrettyString() + ", " + right.ToPrettyString();

                throw new TypeOfExpressionExeption(message, binary.Op);
            }

            public TypeInfo VisitCall(Call call)
            {
                if(call.Callee is Identifier id)
                {
                    List<TypeInfo> funcArgs = call.Args.ConvertAll(a => a.Accept(this));
                    string name = id.Name.Text;
                    if (m_ASTInfo.Functions.TryGetFunction(name, funcArgs, out FunctionInfo funcInfo))
                    {
                        return funcInfo.ReturnType;
                    }
                    else if(m_ASTInfo.Functions.ContainsFunctionWithName(name) && !m_VariableStack.ContainsVariable(name))
                    {
                        string funcMessage = "Function: " + name + " has overload for the specified arguments.";
                        throw new TypeOfExpressionExeption(funcMessage, id.Name);
                    }
                }

                TypeInfo callee = call.Callee.Accept(this);
                List<TypeInfo> args = call.Args.ConvertAll(a => a.Accept(this));
                if (m_OperatorLibrary.CallOperators.TryGet(callee, args, out OperatorInfo.Call info))
                    return info.Returned;

                string message = "No call operator for arguments: " 
                    + args.ConvertAll(a => a.ToPrettyString()).Concat(", ");

                throw new TypeOfExpressionExeption(message, call.OpenParen);
            }

            public TypeInfo VisitCast(Cast cast)
            {
                if(cast.Castee is Identifier id) // casting to find a function overload
                {
                    string name = id.Name.Text;
                    List<FunctionInfo> funcOverloads = m_ASTInfo.Functions.GetOverloadsWithName(name);
                    if(!m_VariableStack.ContainsVariable(name) && funcOverloads.Count > 0)
                    {
                        foreach(FunctionInfo overload in funcOverloads)
                        {
                            TypeInfo.FunctionPointer type = new TypeInfo.FunctionPointer(overload);
                            if (type.Equals(TypeInfo.FromASTType(cast.TypeToCastTo)))
                                return type;
                        }
                    }
                }

                TypeInfo castee = cast.Castee.Accept(this);
                TypeInfo typeToCastTo = TypeInfo.FromASTType(cast.TypeToCastTo);

                if (m_OperatorLibrary.CastOperators.TryGet(typeToCastTo, castee, out OperatorInfo.Cast info))
                    return info.TypeToCastTo;

                string message = "No cast operator for: " + 
                    castee.ToPrettyString() + " to: " + 
                    typeToCastTo.ToPrettyString() + ".";

                throw new TypeOfExpressionExeption(message, cast.AsToken);
            }

            public TypeInfo VisitGrouping(Grouping grouping)
            {
                return grouping.Expr.Accept(this);
            }

            public TypeInfo VisitIdentifier(Identifier identifier)
            {
                string name = identifier.Name.Text;
                if (m_VariableStack.TryGetVariable(name, out VariableInfo info))
                    return info.Type;

                if (m_ASTInfo.GlobalVariables.TryGetValue(name, out info))
                    return info.Type;

                List<FunctionInfo> functions = m_ASTInfo.Functions.GetOverloadsWithName(name);
                if(functions.Count == 1)
                {
                    FunctionInfo funcInfo = functions[0];
                    return new TypeInfo.FunctionPointer(funcInfo);
                }
                else if(functions.Count > 1)
                {
                    string message = "Too many function overloads for function: " + name + " to distinguish.";
                    throw new TypeOfExpressionExeption(message, identifier.Name);
                }

                string varMessage = "Variable: " + name + " is not defined.";
                throw new TypeOfExpressionExeption(varMessage, identifier.Name);
            }

            public TypeInfo VisitIndex(AST.Index index)
            {
                TypeInfo indexed = index.Indexed.Accept(this);
                TypeInfo arg = index.Argument.Accept(this);
                if (m_OperatorLibrary.IndexOperators.TryGet(indexed, arg, out OperatorInfo.Index info))
                    return info.Returned;

                string message = "No index operator for: " + 
                    indexed.ToPrettyString() + 
                    ", with argument: " + 
                    arg.ToPrettyString() + ".";

                throw new TypeOfExpressionExeption(message, index.OpenBracket);
            }

            public TypeInfo VisitInitializerList(InitializerList initializerList)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitLiteral(Literal literal)
            {
                return literal.Val.Type switch
                { 
                    TokenType.IntagerLiteral => RipplePrimitives.Int32,
                    TokenType.FloatLiteral => RipplePrimitives.Float32,
                    TokenType.CharactorLiteral => RipplePrimitives.Char,
                    TokenType.StringLiteral => throw new NotImplementedException(),
                    TokenType.Nullptr => throw new NotImplementedException(),
                    TokenType.True or TokenType.False => RipplePrimitives.Bool,
                    _ => throw new TypeOfExpressionExeption("Unknown literal.", literal.Val)
                };
            }

            public TypeInfo VisitSizeOf(SizeOf sizeOf)
            {
                return RipplePrimitives.Int32;
            }

            public TypeInfo VisitTypeExpression(TypeExpression typeExpression)
            {
                throw new NotImplementedException();
            }

            public TypeInfo VisitUnary(Unary unary)
            {
                TypeInfo operand = unary.Expr.Accept(this);
                TokenType operatorType = unary.Op.Type;

                if (m_OperatorLibrary.UnaryOperators.TryGet(operatorType, operand, out OperatorInfo.Unary info))
                    return info.Returned;

                string message = "No binary operator: " + operatorType.ToString() + ", for type: " +
                    operand.ToPrettyString() + ".";

                throw new TypeOfExpressionExeption(message, unary.Op);
            }
        }

        private class TypeOfExpressionExeption : Exception
        {
            public readonly Token ErrorToken;

            public TypeOfExpressionExeption(string message, Token errorToken) : base(message)
            {
                ErrorToken = errorToken;
            }
        }

    }
}
