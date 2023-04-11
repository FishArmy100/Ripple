using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing.Errors
{
    class EscapeSequenceError : LexerError
    {
        public EscapeSequenceError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Illformated escape sequence.";
    }
}
