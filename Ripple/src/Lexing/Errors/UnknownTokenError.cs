using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;

namespace Ripple.Lexing.Errors
{
    public class UnknownTokenError : LexerError
    {
        public readonly string Token;

        public UnknownTokenError(SourceLocation location, string token) : base(location)
        {
            Token = token;
        }

        public override string GetMessage()
        {
            return $"Unknown token '{Token}'";
        }
    }
}
