using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info
{
    struct ASTBinaryOperator
    {
        public readonly ASTTypeInfo Left;
        public readonly ASTTypeInfo Right;
        public readonly TokenType Operator;
        public readonly ASTTypeInfo ReturnType;

        public ASTBinaryOperator(ASTTypeInfo left, TokenType op, ASTTypeInfo right, ASTTypeInfo returnType)
        {
            Left = left;
            Right = right;
            Operator = op;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            return obj is ASTBinaryOperator op &&
                   EqualityComparer<ASTTypeInfo>.Default.Equals(Left, op.Left) &&
                   EqualityComparer<ASTTypeInfo>.Default.Equals(Right, op.Right) &&
                   Operator == op.Operator &&
                   EqualityComparer<ASTTypeInfo>.Default.Equals(ReturnType, op.ReturnType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right, Operator, ReturnType);
        }
    }
}
