using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils.Extensions
{
    static class NullableTypeExtensions
    {
        public static TOut Match<TIn, TOut>(this TIn? self, Func<TIn, TOut> ok, Func<TOut> fail) where TIn : struct
        {
            if(self is TIn notNull)
            {
                return ok(notNull);
            }

            return fail();
        }
    }
}
