using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;

namespace RippleUnitTests.Lexing
{
    struct ArgumentToken
    {
        public readonly ArgumentTokenType Type;
        public readonly Option<string> Value;

        public ArgumentToken(ArgumentTokenType type, Option<string> value)
        {
            Type = type;
            Value = value;
        }
    }
}
