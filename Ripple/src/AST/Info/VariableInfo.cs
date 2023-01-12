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
using Ripple.AST.Info.Types;

namespace Ripple.AST.Info
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

        public static Result<VariableInfo, List<ASTInfoError>> FromFunctionParameter(TypeName type, Token name, LifetimeInfo lifetime, List<string> primaries, List<LifetimeInfo> lifetimes, SafetyContext safetyContext)
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

        public static Result<List<VariableInfo>, List<ASTInfoError>> FromVarDecl(VarDecl varDecl, ValueOfExpressionVisitor visitor, List<string> primaries, List<LifetimeInfo> lifetimes, LifetimeInfo lifetime, SafetyContext safetyContext)
        {
            if (safetyContext.IsSafe && varDecl.UnsafeToken.HasValue)
                safetyContext = new SafetyContext(false);

            var result = TypeInfoUtils.FromASTType(varDecl.Type, primaries, lifetimes, safetyContext);
            if (result.IsError())
                return result.Error;

            var expressionResult = GetTypeFromExpression(varDecl.Expr, visitor, result.Value);
            return expressionResult.Match(ok =>
            {
                if (!ok.SetFirstMutable(false).IsEquatableTo(result.Value.SetFirstMutable(false)))
                {
                    string varTypeName = TypeNamePrinter.PrintType(varDecl.Type);
                    ASTInfoError error = new ASTInfoError("Cannot assign type '" + ok.ToString() + "', to a variable of type '" + varTypeName + "'.", varDecl.VarNames[0]);
                    return new Result<List<VariableInfo>, List<ASTInfoError>>(new List<ASTInfoError> { error });
                }

                List<VariableInfo> vars = varDecl.VarNames.ConvertAll(n =>
                {
                    return new VariableInfo(n, ok, !safetyContext.IsSafe, lifetime);
                });

                return new Result<List<VariableInfo>, List<ASTInfoError>>(vars);
            },
            fail =>
            {
                return new Result<List<VariableInfo>, List<ASTInfoError>>(fail);
            });
        }

        private static Result<TypeInfo, List<ASTInfoError>> GetTypeFromExpression(Expression expression, ValueOfExpressionVisitor visitor, Option<TypeInfo> expected)
        {
            try
            {
                return expression.Accept(visitor, expected).Type;
            }
            catch (ValueOfExpressionExeption e)
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
