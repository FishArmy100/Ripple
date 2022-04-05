using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class FuncPointerType : RippleType
    {
        public readonly RippleType ReturnType;
        public readonly List<RippleType> Parameters;
        public readonly bool IsNullable;

        public FuncPointerType(RippleType returnType, List<RippleType> parameters, bool isNullable)
        {
            ReturnType = returnType;
            Parameters = parameters;
            IsNullable = isNullable;
        }

        public override void Accept(IRippleTypeVisitor visitor)
        {
            visitor.VisitFuncRefType(this);
        }

        public override T Accept<T>(IRippleTypeVisitor<T> visitor)
        {
            return visitor.VisitFuncRefType(this);
        }
    }
}
