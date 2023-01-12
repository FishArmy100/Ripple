using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info
{
    struct SafetyContext
    {
        public readonly bool IsSafe;

        public SafetyContext(bool isSafe)
        {
            IsSafe = isSafe;
        }

        public override bool Equals(object obj)
        {
            return obj is SafetyContext context &&
                   IsSafe == context.IsSafe;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSafe);
        }
    }
}
