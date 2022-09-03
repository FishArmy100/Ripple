using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    public abstract class Option<T>
    {
        public class Some : Option<T>
        {
            public readonly T Value;
            public Some(T value) { Value = value; }
        }

        public class None : Option<T> { }

        public static Option<T> Good(T value) => new Some(value);
        public static Option<T> Bad() => new None();
    }
}
