using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Parsing.Errors
{
    class ExpectedDeclarationError : ParserError
    {
        public ExpectedDeclarationError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => "Expected a declaration.";
    }
}
