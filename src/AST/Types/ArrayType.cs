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
        public readonly int Dimentions;
        public readonly bool IsNullable;

        public ArrayType(RippleType type, int dimentions, bool isNullable)
        {
            Type = type;
            Dimentions = dimentions;
            IsNullable = isNullable;
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
