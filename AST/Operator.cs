using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct Operator
    {
        public readonly TokenType LeftType;
        public readonly TokenType OperatorType;
        public readonly TokenType RightType;

        public Operator(TokenType leftType, TokenType operatorType, TokenType rightType)
        {
            LeftType = leftType;
            OperatorType = operatorType;
            RightType = rightType;
        }

        public override bool Equals(object obj)
        {
            return obj is Operator op &&
                   LeftType == op.LeftType &&
                   OperatorType == op.OperatorType &&
                   RightType == op.RightType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LeftType, OperatorType, RightType);
        }

        public static bool operator==(Operator left, Operator right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Operator left, Operator right)
        {
            return !left.Equals(right);
        }
    }
}
