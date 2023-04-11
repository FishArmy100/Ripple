using System;
using Ripple.Lexing;
using System.Collections.Generic;
using Ripple.Validation.Errors;

namespace Ripple.Validation.Info
{
    class ExpressionCheckerException : Exception
    {
        public readonly IReadOnlyList<ValidationError> Errors;

        public ExpressionCheckerException(IReadOnlyList<ValidationError> errors)
        {
            Errors = errors;
        }

        public ExpressionCheckerException(ValidationError error)
        {
            Errors = new List<ValidationError> { error };
        }
    }
}
