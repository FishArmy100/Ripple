using System;
using Ripple.Lexing;
using System.Collections.Generic;

namespace Ripple.AST.Info
{
    class ValueOfExpressionExeption : Exception
    {
        public readonly IReadOnlyList<ASTInfoError> Errors;

        public ValueOfExpressionExeption(IReadOnlyList<ASTInfoError> errors)
        {
            Errors = errors;
        }

        public ValueOfExpressionExeption(ASTInfoError error)
        {
            Errors = new List<ASTInfoError> { error };
        }

        public ValueOfExpressionExeption(string message, Token errorToken)
        {
            Errors = new List<ASTInfoError> { new ASTInfoError(message, errorToken) };
        }
    }
}
