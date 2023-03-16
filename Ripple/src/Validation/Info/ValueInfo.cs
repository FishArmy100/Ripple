using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Utils;
using Ripple.Utils;
using Ripple.Lexing;
using Ripple.Validation.Info.Types;
using Ripple.AST;

namespace Ripple.Validation.Info
{
    class ValueInfo
    {
        public readonly TypeInfo Type;
        public readonly LifetimeInfo Lifetime;

        public ValueInfo(TypeInfo type, LifetimeInfo lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueInfo info &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type) &&
                   EqualityComparer<LifetimeInfo>.Default.Equals(Lifetime, info.Lifetime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Lifetime);
        }

        public override string ToString()
        {
            return Type.ToPrettyString() + ":(" + Lifetime.ToString() + ")";
        }
    }
}
