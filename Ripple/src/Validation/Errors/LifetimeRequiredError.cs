using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class LifetimeRequiredError : ValidationError
    {
        public LifetimeRequiredError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "A lifetime is required in this context.";
    }
}
