using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct ASTUnaryOperator
    {
        public readonly ASTType Type;
        public readonly TokenType Operator;
        public readonly ASTType ReturnType;

        public ASTUnaryOperator(ASTType type, TokenType op, ASTType returnType)
        {
            Type = type;
            Operator = op;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            return obj is ASTUnaryOperator op &&
                   EqualityComparer<ASTType>.Default.Equals(Type, op.Type) &&
                   Operator == op.Operator &&
                   EqualityComparer<ASTType>.Default.Equals(ReturnType, op.ReturnType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Operator, ReturnType);
        }
    }
}
