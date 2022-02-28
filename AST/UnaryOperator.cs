using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct UnaryOperator
    {
        public readonly ASTType Type;
        public readonly ASTType ReturnType;
        public readonly TokenType OperatorType;

        public UnaryOperator(ASTType type, ASTType returnType, TokenType operatorToken)
        {
            Type = type;
            OperatorType = operatorToken;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op &&
                   Type == op.Type &&
                   ReturnType == op.ReturnType &&
                   OperatorType == op.OperatorType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, ReturnType, OperatorType);
        }

        public static bool operator ==(UnaryOperator left, UnaryOperator right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UnaryOperator left, UnaryOperator right)
        {
            return !left.Equals(right);
        }
    }
}
