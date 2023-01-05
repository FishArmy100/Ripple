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

        public static Result<VariableInfo, List<ASTInfoError>> FromFunctionParameter(TypeName type, Token name, LifetimeInfo lifetime, List<PrimaryTypeInfo> primaries, List<Token> lifetimes, SafetyContext safetyContext)
        {
            List<ASTInfoError> typeNameErrors = new TypeNameValidityChecker(type, primaries, lifetimes, safetyContext).Errors;
            if (typeNameErrors.Count > 0)
                return typeNameErrors;

            return TypeInfo.FromASTType(type, primaries, lifetimes, safetyContext).ToResult().Match(ok =>
            {
                VariableInfo info = new VariableInfo(name, ok, !safetyContext.IsSafe, lifetime);
                return new Result<VariableInfo, List<ASTInfoError>>(info);
            },
            fail =>
            {
                return new Result<VariableInfo, List<ASTInfoError>>(fail);
            });
        }

        public static Result<List<VariableInfo>, List<ASTInfoError>> FromVarDecl(VarDecl varDecl, ValueOfExpressionVisitor visitor, List<PrimaryTypeInfo> primaries, List<Token> lifetimes, LifetimeInfo lifetime, SafetyContext safetyContext)
        {
            if (safetyContext.IsSafe && varDecl.UnsafeToken.HasValue)
                safetyContext = new SafetyContext(false);

            TypeInfoCreationResult creationResult = TypeInfo.FromASTType(varDecl.Type, primaries, lifetimes, safetyContext);

            if (creationResult.InvalidTypeErrors.Count > 0)
                return creationResult.InvalidTypeErrors;

            var expressionResult = GetTypeFromExpression(varDecl.Expr, visitor, creationResult.Type);
            return expressionResult.Match(ok =>
            {
                bool wasMutable = varDecl.Type switch // sets the first type to immutable
                {
                    PointerType p => p.MutToken.HasValue,
                    ReferenceType r => r.MutToken.HasValue,
                    FuncPtr fp => fp.MutToken.HasValue,
                    ArrayType a => a.MutToken.HasValue,
                    BasicType b => b.MutToken.HasValue,
                    _ => throw new ArgumentException("Unknown type name " + varDecl.Type.ToString())
                };

                TypeName changedMut = varDecl.Type switch // sets the first type to immutable
                { 
                    PointerType p => new PointerType(p.BaseType, null, p.Star),
                    ReferenceType r => new ReferenceType(r.BaseType, null, r.Ampersand, r.Lifetime),
                    FuncPtr fp => new FuncPtr(null, fp.FuncToken, fp.Lifetimes, fp.OpenParen, fp.Parameters, fp.CloseParen, fp.Arrow, fp.ReturnType),
                    ArrayType a => new ArrayType(a.BaseType, null, a.OpenBracket, a.Size, a.CloseBracket),
                    BasicType b => new BasicType(null, b.Identifier),
                    _ => throw new ArgumentException("Unknown type name " + varDecl.Type.ToString())
                };

                if (!ok.ChangeMutable(false).IsEquatableToTypeName(changedMut))
                {
                    string varTypeName = TypeNamePrinter.PrintType(varDecl.Type);
                    ASTInfoError error = new ASTInfoError("Cannot assign type '" + ok.ToString() + "', to a variable of type '" + varTypeName + "'.", varDecl.VarNames[0]);
                    return new Result<List<VariableInfo>, List<ASTInfoError>>(new List<ASTInfoError> { error });
                }

                List<VariableInfo> vars = varDecl.VarNames.ConvertAll(n =>
                {
                    return new VariableInfo(n, ok.ChangeMutable(wasMutable), !safetyContext.IsSafe, lifetime);
                });

                return new Result<List<VariableInfo>, List<ASTInfoError>>(vars);
            },
            fail =>
            {
                return new Result<List<VariableInfo>, List<ASTInfoError>>(new List<ASTInfoError> { fail });
            });
        }

        private static Result<TypeInfo, ASTInfoError> GetTypeFromExpression(Expression expression, ValueOfExpressionVisitor visitor, Option<TypeInfo> expected)
        {
            try
            {
                return expression.Accept(visitor, expected).Type;
            }
            catch (AmbiguousTypeException e)
            {
                return new ASTInfoError(e.Message, e.ErrorToken);
            }
            catch (TypeOfExpressionExeption e)
            {
                return new ASTInfoError(e.Message, e.ErrorToken);
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
