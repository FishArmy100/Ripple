using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ripple.Lexing;

namespace Ripple.Parsing
{
    class ParserExeption : Exception
    {
        public Token Tok;

        public ParserExeption(Token tok, string message) : base(message)
        {
            Tok = tok;
        }
    }
}
