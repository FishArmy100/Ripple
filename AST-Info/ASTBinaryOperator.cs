using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct ASTBinaryOperator
    {
        public readonly ASTType Left;
        public readonly ASTType Right;
        public readonly TokenType Operator;
        public readonly ASTType ReturnType;

        public ASTBinaryOperator(ASTType left, TokenType op, ASTType right, ASTType returnType)
        {
            Left = left;
            Right = right;
            Operator = op;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            return obj is ASTBinaryOperator op &&
                   EqualityComparer<ASTType>.Default.Equals(Left, op.Left) &&
                   EqualityComparer<ASTType>.Default.Equals(Right, op.Right) &&
                   Operator == op.Operator &&
                   EqualityComparer<ASTType>.Default.Equals(ReturnType, op.ReturnType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right, Operator, ReturnType);
        }
    }
}
