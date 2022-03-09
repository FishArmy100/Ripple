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
        public readonly string ErrorMessage;
        public readonly Token? Token;
        public readonly int Line;

        public bool HasToken => Token.HasValue;

        public CompilerError(ParserError error)
        {
            ErrorMessage = error.Message;
            Token = error.Token;
            Line = error.Token.Line;
        }

        public CompilerError(ScannerError error)
        {
            ErrorMessage = error.Message;
            Token = null;
            Line = error.Line;
        }
    }
}
