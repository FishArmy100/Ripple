using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils.Extensions
{
    static class CharExtensions
    {
        public static bool IsNumeric(this char c)
        {
            return char.IsDigit(c);
        }

        public static bool IsAlpha(this char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        public static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }

        public static bool IsAlphaNumeric(this char c)
        {
            return c.IsAlpha() || c.IsNumeric();
        }

        public static bool IsWhiteSpace(this char c)
        {
            return char.IsWhiteSpace(c);
        }
    }
}
