using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class ArrayType : RippleType
    {
        public readonly RippleType Type;
        public readonly RippleArrayTypePostfix Postifix;

        public ArrayType(RippleType type, RippleArrayTypePostfix postfix)
        {
            Type = type;
            Postifix = postfix;
        }

        public override void Accept(IRippleTypeVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override T Accept<T>(IRippleTypeVisitor<T> visitor)
        {
            return visitor.VisitArrayType(this);
        }
    }
}
