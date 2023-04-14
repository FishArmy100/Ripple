using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;
using Ripple.AST.Utils;
using Raucse;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Errors;
using Ripple.Validation.Errors.ExpressionErrors;
using Ripple.Core;

namespace Ripple.Validation.Info
{
    public class VariableInfo
    {
        public readonly Token NameToken;
        public readonly TypeInfo Type;
        public readonly bool IsUnsafe;
        public readonly LifetimeInfo Lifetime;
        public readonly bool IsMutable;

        public bool IsGlobal => Lifetime.Equals(LifetimeInfo.Static);

        private VariableInfo(Token nameToken, TypeInfo type, bool isUnsafe, LifetimeInfo lifetime, bool isMutable)
        {
            NameToken = nameToken;
            Type = type;
            IsUnsafe = isUnsafe;
            Lifetime = lifetime;
            IsMutable = isMutable;
        }

        public static Result<VariableInfo, List<ValidationError>> FromFunctionParameter(TypeName type, Token name, LifetimeInfo lifetime, IReadOnlyList<string> primaries, List<string> lifetimes, SafetyContext safetyContext)
        {
            return TypeInfoUtils.FromASTType(type, primaries, lifetimes, safetyContext, true).Match(ok =>
            {
                VariableInfo info = new VariableInfo(name, ok, !safetyContext.IsSafe, lifetime, false);
                return new Result<VariableInfo, List<ValidationError>>(info);
            },
            fail =>
            {
                return new Result<VariableInfo, List<ValidationError>>(fail);
            });
        }

        public static Result<Pair<List<VariableInfo>, TypedExpression>, List<ValidationError>> FromVarDecl(VarDecl varDecl, ExpressionCheckerVisitor visitor, IReadOnlyList<string> primaries, List<string> lifetimes, LifetimeInfo lifetime, SafetyContext safetyContext)
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
                if (!type.IsEquatableTo(result.Value))
                {
                    string varTypeName = TypeNamePrinter.PrintType(varDecl.Type);
                    SourceLocation location = varDecl.Type.GetLocation() + varDecl.VarNames.Select(v => v.Location).Sum();
                    ValidationError error = new AssignmentError(location, result.Value, type);
                    return new Result<Pair<List<VariableInfo>, TypedExpression>, List<ValidationError>>(new List<ValidationError> { error });
                }

                List<VariableInfo> vars = varDecl.VarNames.ConvertAll(n =>
                {
                    return new VariableInfo(n, type, !safetyContext.IsSafe, lifetime, varDecl.MutToken.HasValue);
                });

                var pair = new Pair<List<VariableInfo>, TypedExpression>(vars, ok.Second);
                return new Result<Pair<List<VariableInfo>, TypedExpression>, List<ValidationError>>(pair);
            },
            fail =>
            {
                return new Result<Pair<List<VariableInfo>, TypedExpression>, List<ValidationError>>(fail);
            });
        }

        private static Result<Pair<ValueInfo, TypedExpression>, List<ValidationError>> CheckExpression(Expression expression, ExpressionCheckerVisitor visitor, Option<TypeInfo> expected)
        {
            try
            {
                Option<ExpectedValue> expectedVal = expected.MatchOrConstruct(e => new Option<ExpectedValue>(new ExpectedValue(e, false)));
                return expression.Accept(visitor, expectedVal);
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
