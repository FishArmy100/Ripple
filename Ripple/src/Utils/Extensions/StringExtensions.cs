using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils.Extensions
{
    static class StringExtensions
    {
        public static string AppendLine(this string self, string str)
        {
            return self + str + "\n";
        }

        public static string AppendLine(this string self, string str, int indent)
        {
            return self.AppendLine(StringUtils.GenIndent(indent) + str);
        }

        public static string Concat(this IEnumerable<string> list, string seperator = " ")
        {
            string str = "";
            for(int i = 0; i < list.Count(); i++)
            {
                if (i != 0)
                    str += seperator;

                str += list.ElementAt(i);
            }

            return str;
        }

        public static string Concat(this IEnumerable<string> list, Func<string, string> modifier, string seperator = " ")
        {
            string str = "";
            for (int i = 0; i < list.Count(); i++)
            {
                if (i != 0)
                    str += seperator;

                str += list.ElementAt(i);
            }

            return str;
        }

        public static string FirstCharToLowerCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
        }

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
