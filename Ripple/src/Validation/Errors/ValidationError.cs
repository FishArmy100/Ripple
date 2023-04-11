using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Compiling;
using Ripple.Core;

namespace Ripple.Validation.Errors
{
    public abstract class ValidationError : CompilerError
    {
        protected ValidationError(SourceLocation location) : base(location)
        {
        }
    }
}
