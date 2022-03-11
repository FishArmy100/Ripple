using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple
{
    struct CompilerError
    {
        public readonly string FilePath;
        public readonly int Line;
        public readonly int Column;
        public readonly string Message;

        public CompilerError(string filePath, int line, int column, string message)
        {
            FilePath = filePath;
            Line = line;
            Column = column;
            Message = message;
        }

        public override string ToString()
        {
            return FilePath + ":" + Line + ":" + Column + ": " + Message;
        }
    }
}
