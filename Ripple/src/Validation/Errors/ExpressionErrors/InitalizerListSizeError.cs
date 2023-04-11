using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class InitalizerListSizeError : ValidationError
    {
        public InitalizerListSizeError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Initializer list size is bigger than the array size.";
    }
}
