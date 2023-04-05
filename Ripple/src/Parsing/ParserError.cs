using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Parsing
{
    public struct ParserError
    {
        public string Message;
        public Token Tok;

        public ParserError(string message, Token tok)
        {
            Message = message;
            Tok = tok;
        }
    }
}
