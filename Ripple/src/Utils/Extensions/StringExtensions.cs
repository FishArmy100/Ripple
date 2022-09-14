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
    }
}
