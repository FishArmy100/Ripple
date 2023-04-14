using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Parsing.Errors;
using Ripple.Lexing;

namespace Ripple.Parsing
{
    class ParserExeption : Exception
    {
        public readonly ParserError Error;

        public ParserExeption(ParserError error) : base(error.GetMessage())
        {
            Error = error;
        }
    }
}
