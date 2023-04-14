using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Validation;
using Ripple.Core;

namespace Ripple.Compiling
{
    public abstract class CompilerError
    {
        public readonly SourceLocation Location;

        protected CompilerError(SourceLocation location)
        {
            Location = location;
        }

        public abstract string GetMessage();
    }
}
