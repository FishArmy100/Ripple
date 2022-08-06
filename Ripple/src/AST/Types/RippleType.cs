using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    abstract class RippleType
    {
        public abstract void Accept(IRippleTypeVisitor visitor);
        public abstract T Accept<T>(IRippleTypeVisitor<T> visitor);
    }
}
