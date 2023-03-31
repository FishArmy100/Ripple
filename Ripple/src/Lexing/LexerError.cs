using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing
{
    public struct LexerError
    {
        public readonly string Message;
        public readonly int Line;
        public readonly int Column;

        public LexerError(string message, int line, int column)
        {
            Message = message;
            Line = line;
            Column = column;
        }
    }
}
