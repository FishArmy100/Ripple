using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct BinaryOperator
    {
        public readonly ASTType LeftType;
        public readonly TokenType OperatorType;
        public readonly ASTType RightType;
        public readonly ASTType ReturnType;

        public BinaryOperator(ASTType leftToken, TokenType operatorToken, ASTType rightToken, ASTType returnType)
        {
            LeftType = leftToken;
            OperatorType = operatorToken;
            RightType = rightToken;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryOperator op &&
                   LeftType == op.LeftType &&
                   OperatorType == op.OperatorType &&
                   RightType == op.RightType &&
                   ReturnType == op.ReturnType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LeftType, OperatorType, RightType, ReturnType);
        }

        public static bool operator==(BinaryOperator left, BinaryOperator right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BinaryOperator left, BinaryOperator right)
        {
            return !left.Equals(right);
        }
    }
}
