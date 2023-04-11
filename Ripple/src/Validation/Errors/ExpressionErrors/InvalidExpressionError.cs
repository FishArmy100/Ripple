using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class InvalidExpressionError : ValidationError
    {
        public InvalidExpressionError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Invalid expression at call.";
    }
}
