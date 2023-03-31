using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Lexing
{
    static class TokenTypeExtensions
    {
        public static bool IsKeyword(this TokenType type)
        {
            return RippleKeywords.Keywords.ContainsValue(type);
        }

        public static bool IsIdentifier(this TokenType type)
        {
            return type == TokenType.Identifier;
        }

        public static bool IsType(this TokenType type, params TokenType[] types)
        {
            return types.Contains(type);
        }
    }
}
