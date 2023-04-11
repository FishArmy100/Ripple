using Ripple.Core;
using Ripple.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Parsing.Errors
{
    class CannotHaveSpaceError : ParserError
    {
        public readonly TokenType Type;
        public CannotHaveSpaceError(SourceLocation location, TokenType type) : base(location)
        {
            Type = type;
        }

        public override string GetMessage() => $"Cannot have a space after '{Type.ToPrettyString()}'";
    }
}
