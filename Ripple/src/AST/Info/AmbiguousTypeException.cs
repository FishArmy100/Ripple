using System;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class AmbiguousTypeException : Exception
    {
        public readonly Token ErrorToken;

        public AmbiguousTypeException(string message, Token errorToken) : base(message)
        {
            ErrorToken = errorToken;
        }
    }
}
