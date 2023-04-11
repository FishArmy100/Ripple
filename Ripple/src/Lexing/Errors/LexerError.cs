using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Compiling;
using Ripple.Core;

namespace Ripple.Lexing
{
    public abstract class LexerError : CompilerError
    {
        protected LexerError(SourceLocation location) : base(location)
        {
        }
    }
}
