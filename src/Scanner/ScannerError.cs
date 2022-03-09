using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    struct ScannerError
    {
        public readonly string Message;
        public readonly int Line;
        public readonly string Where;

        public ScannerError(string message, int line, string where = "")
        {
            Message = message;
            Line = line;
            Where = where;
        }

        public override string ToString()
        {
            return "[line: " + Line + "] Error" + Where + ": " + Message;
        }
    }
}
