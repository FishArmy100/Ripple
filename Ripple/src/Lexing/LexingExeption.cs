using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing
{
    class LexingExeption : Exception
    {
        public readonly int Line;
        public readonly int Column;

        public LexingExeption(string message, int line, int column) : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}
