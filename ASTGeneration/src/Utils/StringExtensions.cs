using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ASTGeneration.Utils.Extensions
{
    static class StringExtensions
    {
        public static string FirstCharToLowerCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
        }

        public static string Concat(this IEnumerable<string> list, string seperator = " ")
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

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
