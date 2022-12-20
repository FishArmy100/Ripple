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
        public readonly LifetimeInfo Liftime;

        public ValueInfo(TypeInfo type, LifetimeInfo liftime)
        {
            Type = type;
            Liftime = liftime;
        }

        public ValueInfo(TypeInfo type, int lifetime) : this(type, new LifetimeInfo(lifetime)) { }

        public static Result<ValueInfo, ASTInfoError> FromExpression(ASTInfo ast, LocalVariableStack localVariables, Expression expr, Option<TypeInfo> expected)
        {
            ValueOfExpressionVisitor visitor = new ValueOfExpressionVisitor(ast, localVariables);
            try
            {
                return expr.Accept(visitor, expected);
            }
            catch (TypeOfExpressionExeption e)
            {
                return new ASTInfoError(e.Message, e.ErrorToken);
            }
            catch (AmbiguousTypeException e)
            {
                return new ASTInfoError(e.Message, e.ErrorToken);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is ValueInfo info &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type) &&
                   EqualityComparer<LifetimeInfo>.Default.Equals(Liftime, info.Liftime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Liftime);
        }
    }
}
