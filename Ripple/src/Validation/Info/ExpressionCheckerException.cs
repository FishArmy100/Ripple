using System;
using Ripple.Lexing;
using System.Collections.Generic;

namespace Ripple.Validation.Info
{
    class ExpressionCheckerException : Exception
    {
        public readonly IReadOnlyList<ASTInfoError> Errors;

        public ExpressionCheckerException(IReadOnlyList<ASTInfoError> errors)
        {
            Errors = errors;
        }

        public ExpressionCheckerException(ASTInfoError error)
        {
            Errors = new List<ASTInfoError> { error };
        }

        public ExpressionCheckerException(string message, Token errorToken)
        {
            Errors = new List<ASTInfoError> { new ASTInfoError(message, errorToken) };
        }
    }
}
