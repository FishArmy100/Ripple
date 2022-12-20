using System;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class TypeOfExpressionExeption : Exception
    {
        public readonly Token ErrorToken;

        public TypeOfExpressionExeption(string message, Token errorToken) : base(message)
        {
            ErrorToken = errorToken;
        }
    }
}
