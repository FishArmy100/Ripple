using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    static class StringUtils
    {
        public static string GenIndent(int count)
        {
            return new string('\t', count);
        }
    }
}
