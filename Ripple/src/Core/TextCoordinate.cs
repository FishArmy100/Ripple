using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Core
{
    public struct TextCoordinate
    {
        public readonly int Line;
        public readonly int Row;

        public TextCoordinate(int line, int row)
        {
            Line = line;
            Row = row;
        }

        public static TextCoordinate FromIndex(string src, int index)
        {
            string subString = src.Substring(0, index);
            int line = subString.Count(c => c == '\n'); // Count the number of line breaks
            int row = subString.Length - subString.LastIndexOf('\n') - 1; //Calculate number of chars since last line break, adjust of off-by-one error

            return new TextCoordinate(line + 1, row + 1);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Line}:{Row}";
        }

        public static bool operator ==(TextCoordinate left, TextCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextCoordinate left, TextCoordinate right)
        {
            return !(left == right);
        }
    }
}
