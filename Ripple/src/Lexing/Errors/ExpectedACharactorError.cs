using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing.Errors
{
    class ExpectedACharactorError : LexerError
    {
        public ExpectedACharactorError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Expected a charactor.";
    }
}
