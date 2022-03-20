using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info
{
    struct ASTUnaryOperator
    {
        public readonly ASTTypeInfo Type;
        public readonly TokenType Operator;
        public readonly ASTTypeInfo ReturnType;

        public ASTUnaryOperator(ASTTypeInfo type, TokenType op, ASTTypeInfo returnType)
        {
            Type = type;
            Operator = op;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            return obj is ASTUnaryOperator op &&
                   EqualityComparer<ASTTypeInfo>.Default.Equals(Type, op.Type) &&
                   Operator == op.Operator &&
                   EqualityComparer<ASTTypeInfo>.Default.Equals(ReturnType, op.ReturnType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Operator, ReturnType);
        }
    }
}
