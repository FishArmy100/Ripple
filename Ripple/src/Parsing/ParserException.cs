using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple.Parsing
{
    class ParserException : Exception
    {
        public readonly ParserError Error;

        public ParserException(ParserError error) : base(error.Message)
        {
            Error = error;
        }

        public ParserException(string message, Token token, int index) : base(message)
        {
            Error = new ParserError(message, token, index);
        }
    }
}
