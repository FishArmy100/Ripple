using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class OverloadDisambiguationError : ValidationError
    {
        public OverloadDisambiguationError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Cannot disambiguate between given function overloads.";
    }
}
