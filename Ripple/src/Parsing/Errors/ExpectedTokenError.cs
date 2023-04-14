using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Lexing;
using Raucse.Extensions;

namespace Ripple.Parsing.Errors
{
    public class ExpectedTokenError : ParserError
    {
        public readonly TokenType[] Types;

        public ExpectedTokenError(SourceLocation location, params TokenType[] types) : base(location)
        {
            if (types.Length == 0)
                throw new ArgumentException("Must pass at least one TokenType");

            Types = types;
        }

        public override string GetMessage() => $"Expected {Types.Select(t => "\'" + t.ToPrettyString() + "\'").Concat(", ")} token";
    }
}
