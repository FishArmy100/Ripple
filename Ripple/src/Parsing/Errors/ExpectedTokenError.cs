using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Lexing;

namespace Ripple.Parsing.Errors
{
    public class ExpectedTokenError : ParserError
    {
        public readonly TokenType Type;

        public ExpectedTokenError(SourceLocation location, TokenType type) : base(location)
        {
            Type = type;
        }

        public override string GetMessage() => $"Expected token '{Type.ToPrettyString()}'";
    }
}
