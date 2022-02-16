using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    struct Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object Literal;
        public readonly int Line;

        public bool IsLiteral => Literal != null;
        public Token Invalid => new Token(TokenType.Invalid, null, 0);

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public Token(TokenType type, string lexeme, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = null;
            Line = line;
        }

        public override string ToString()
        {
            if (IsLiteral)
                return Type + ": \"" + Lexeme + "\" (Literal = " + Literal + ")";

            return Type + ": \"" + Lexeme + "\"";
        }
    }
}
