using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple.Parsing
{
    struct ParserError
    {
        public readonly string Message;
        public readonly Token Token;
        public readonly int Index;

        public ParserError(string message, Token token, int index)
        {
            Message = message;
            Token = token;
            Index = index;
        }
    }
}
