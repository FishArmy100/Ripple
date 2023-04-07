using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class ReturnedFromAVoidFunctionError : ValidationError
    {
        public ReturnedFromAVoidFunctionError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => 
            "Cannot return a value from a void function";
    }
}
