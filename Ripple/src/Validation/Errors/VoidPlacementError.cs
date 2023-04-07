using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class VoidPlacementError : ValidationError
    {
        public VoidPlacementError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Void can only be used in the context of a return type, or as a void*.";
    }
}
