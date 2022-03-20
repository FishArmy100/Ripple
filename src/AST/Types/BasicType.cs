using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class BasicType : RippleType
    {
        public readonly Token Name;
        public readonly bool IsReference;
        public readonly bool IsNullable;

        public BasicType(Token name, bool isReference, bool isNullable)
        {
            Name = name;
            IsReference = isReference;
            IsNullable = isNullable;
        }

        public override void Accept(IRippleTypeVisitor visitor)
        {
            visitor.VisitBasicType(this);
        }

        public override T Accept<T>(IRippleTypeVisitor<T> visitor)
        {
            return visitor.VisitBasicType(this);
        }
    }
}
