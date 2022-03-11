using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    struct ScannerError
    {
        public readonly string Message;
        public readonly int Line;
        public readonly int Column;
        public readonly string Where;

        public ScannerError(string message, int line, int column, string where = "")
        {
            Message = message;
            Line = line;
            Where = where;
            Column = column;
        }
    }
}
