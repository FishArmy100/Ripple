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
using Ripple.Validation.Info.Lifetimes;

namespace Ripple.Validation.Info.Values
{
    public class ValueInfo
    {
        public readonly TypeInfo Type;
        public readonly LifetimeInfo Lifetime;
        public readonly bool IsMutable;
        public readonly ValueCatagory Catagory;

        public ValueInfo(TypeInfo type, LifetimeInfo lifetime, bool isMutable, ValueCatagory catagory)
        {
            Type = type;
            Lifetime = lifetime;
            IsMutable = isMutable;
            Catagory = catagory;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueInfo info &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type) &&
                   EqualityComparer<LifetimeInfo>.Default.Equals(Lifetime, info.Lifetime) &&
                   IsMutable == info.IsMutable &&
                   Catagory == info.Catagory;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Lifetime, IsMutable, Catagory);
        }

        public override string ToString()
        {
            return Type.ToPrettyString() + (IsMutable ? " mut" : "") + ":(" + Lifetime.ToString() + ")";
        }
    }
}
