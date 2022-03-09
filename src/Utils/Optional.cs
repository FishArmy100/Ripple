using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    struct Optional<T>
    {
        public readonly T Value;
        public readonly bool HasValue;

        public Optional(T value)
        {
            Value = value;
            HasValue = true;
        }

        public static implicit operator bool(Optional<T> optional) => optional.HasValue;
        public static implicit operator T(Optional<T> optional) => optional.Value;
    }

    struct Optional<T, TNone>
    {
        public readonly T Value;
        public readonly TNone NoneValue;
        public readonly bool HasValue;
        public readonly bool HasNoneValue;

        public Optional(T value)
        {
            Value = value;
            HasValue = true;
            NoneValue = default;
            HasNoneValue = false;
        }

        public Optional(TNone noneValue)
        {
            Value = default;
            HasValue = false;
            NoneValue = noneValue;
            HasNoneValue = true;
        }

        public static implicit operator bool(Optional<T, TNone> optional) => optional.HasValue;
        public static implicit operator T(Optional<T, TNone> optional) => optional.Value;
        public static implicit operator TNone(Optional<T, TNone> optional) => optional.NoneValue;
    }
}
