using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Utils;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class ValueInfo
    {
        public readonly TypeInfo Type;
        public readonly LifetimeInfo Lifetime;

        public ValueInfo(TypeInfo type, LifetimeInfo lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public ValueInfo(TypeInfo type, int lifetime) : this(type, new LifetimeInfo(lifetime)) { }

        public static Result<ValueInfo, ASTInfoError> FromExpression(Expression expression, ASTInfo ast, LocalVariableStack variableStack, Option<TypeInfo> expected = default)
        {
            ValueOfExpressionVisitor visitor = new ValueOfExpressionVisitor(ast, variableStack);
            return FromExpression(expression, visitor, expected);
        }

        public static Result<ValueInfo, ASTInfoError> FromExpression(Expression expression, ValueOfExpressionVisitor visitor, Option<TypeInfo> expected = default)
        {
            try
            {
                return expression.Accept(visitor, expected);
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

        public override bool Equals(object obj)
        {
            return obj is ValueInfo info &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type) &&
                   EqualityComparer<LifetimeInfo>.Default.Equals(Lifetime, info.Lifetime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Lifetime);
        }

        public override string ToString()
        {
            return Type.ToString() + ":(" + Lifetime.ToString() + ")";
        }
    }
}
