using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Info
{
    public struct SafetyContext
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

        public static bool operator ==(SafetyContext left, SafetyContext right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SafetyContext left, SafetyContext right)
        {
            return !(left == right);
        }
    }
}
