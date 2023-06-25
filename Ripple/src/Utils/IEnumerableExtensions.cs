using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    static class IEnumerableExtensions
    {
        public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T> enumerable)
        {
            return enumerable
                .GroupBy(i => i)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
        }
    }
}
