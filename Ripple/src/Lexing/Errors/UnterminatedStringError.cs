using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;

namespace Ripple.Lexing.Errors
{
    class UnterminatedStringError : LexerError
    {
        public UnterminatedStringError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Unterminated string";
    }
}
