using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Info.Values
{
    public class ExpectedValue 
    {
        public readonly TypeInfo Type;
        public readonly bool IsMutable;

        public ExpectedValue(TypeInfo type, bool isMutable)
        {
            Type = type;
            IsMutable = isMutable;
        }

        public override bool Equals(object obj)
        {
            return obj is ExpectedValue value &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, value.Type) &&
                   IsMutable == value.IsMutable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, IsMutable);
        }
    }
}
