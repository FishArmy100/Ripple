using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class ConditionalValueError : ValidationError
    {
        public ConditionalValueError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Conditional must evaluate to a bool.";
    }
}
