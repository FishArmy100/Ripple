using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class NoFunctionFoundError : ValidationError
    {
        public NoFunctionFoundError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "No function with given arguments found.";
    }
}
