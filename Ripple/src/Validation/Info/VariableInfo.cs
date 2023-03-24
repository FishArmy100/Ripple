using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;
using Ripple.AST.Utils;
using Ripple.Utils;
using Ripple.Utils.Extensions;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info.Expressions;

namespace Ripple.Validation.Info
{
    class VariableInfo
    {
        public readonly Token NameToken;
        public readonly TypeInfo Type;
        public readonly bool IsUnsafe;
        public readonly LifetimeInfo Lifetime;

        public bool IsGlobal => Lifetime.Equals(LifetimeInfo.Static);

        private VariableInfo(Token nameToken, TypeInfo type, bool isUnsafe, LifetimeInfo lifetime)
        {
            NameToken = nameToken;
            Type = type;
            IsUnsafe = isUnsafe;
            Lifetime = lifetime;
        }

        public static Result<VariableInfo, List<ASTInfoError>> FromFunctionParameter(TypeName type, Token name, LifetimeInfo lifetime, IReadOnlyList<string> primaries, List<string> lifetimes, SafetyContext safetyContext)
        {
            return TypeInfoUtils.FromASTType(type, primaries, lifetimes, safetyContext, true).Match(ok =>
            {
                VariableInfo info = new VariableInfo(name, ok, !safetyContext.IsSafe, lifetime);
                return new Result<VariableInfo, List<ASTInfoError>>(info);
            },
            fail =>
            {
                return new Result<VariableInfo, List<ASTInfoError>>(fail);
            });
        }

        public static Result<Pair<List<VariableInfo>, TypedExpression>, List<ASTInfoError>> FromVarDecl(VarDecl varDecl, ExpressionCheckerVisitor visitor, IReadOnlyList<string> primaries, List<string> lifetimes, LifetimeInfo lifetime, SafetyContext safetyContext)
        {
            if (safetyContext.IsSafe && varDecl.UnsafeToken.HasValue)
                safetyContext = new SafetyContext(false);

            var result = TypeInfoUtils.FromASTType(varDecl.Type, primaries, lifetimes, safetyContext);
            if (result.IsError())
                return result.Error;

            var expressionResult = CheckExpression(varDecl.Expr, visitor, result.Value);
            return expressionResult.Match(ok =>
            {
                TypeInfo type = ok.First.Type;
                if (!type.SetFirstMutable(false).IsEquatableTo(result.Value.SetFirstMutable(false)))
                {
                    string varTypeName = TypeNamePrinter.PrintType(varDecl.Type);
                    ASTInfoError error = new ASTInfoError("Cannot assign type '" + type.ToString() + "', to a variable of type '" + varTypeName + "'.", varDecl.VarNames[0]);
                    return new Result<Pair<List<VariableInfo>, TypedExpression>, List<ASTInfoError>>(new List<ASTInfoError> { error });
                }

                List<VariableInfo> vars = varDecl.VarNames.ConvertAll(n =>
                {
                    return new VariableInfo(n, type, !safetyContext.IsSafe, lifetime);
                });

                var pair = new Pair<List<VariableInfo>, TypedExpression>(vars, ok.Second);
                return new Result<Pair<List<VariableInfo>, TypedExpression>, List<ASTInfoError>>(pair);
            },
            fail =>
            {
                return new Result<Pair<List<VariableInfo>, TypedExpression>, List<ASTInfoError>>(fail);
            });
        }

        private static Result<Pair<ValueInfo, TypedExpression>, List<ASTInfoError>> CheckExpression(Expression expression, ExpressionCheckerVisitor visitor, Option<TypeInfo> expected)
        {
            try
            {
                return expression.Accept(visitor, expected);
            }
            catch (ExpressionCheckerException e)
            {
                return e.Errors.ToList();
            }
        }

        public string Name => NameToken.Text;

        public override bool Equals(object obj)
        {
            return obj is VariableInfo info &&
                   EqualityComparer<Token>.Default.Equals(NameToken, info.NameToken) &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type) &&
                   IsUnsafe == info.IsUnsafe &&
                   Lifetime == info.Lifetime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NameToken, Type, IsUnsafe, Lifetime);
        }
    }
}
