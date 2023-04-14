using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing.Errors
{
    class LifetimeNameError : LexerError
    {
        public LifetimeNameError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Lifetime must start with an alpha charactor.";
    }
}
