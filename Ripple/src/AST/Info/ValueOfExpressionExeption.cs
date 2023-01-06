using System;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class ValueOfExpressionExeption : Exception
    {
        public readonly Token ErrorToken;

        public ValueOfExpressionExeption(string message, Token errorToken) : base(message)
        {
            ErrorToken = errorToken;
        }
    }
}
