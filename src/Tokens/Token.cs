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
        public readonly int Column;

        public bool IsLiteral => Literal != null;
        public static Token Invalid => new Token(TokenType.Invalid, null, 0, 0);

        public Token(TokenType type, string lexeme, object literal, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
            Column = column;
        }

        public Token(TokenType type, string lexeme, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = null;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            if (IsLiteral)
                return Type + ": \"" + Lexeme + "\" (Literal = " + Literal + ")";

            return Type + ": \"" + Lexeme + "\"";
        }

        public override bool Equals(object obj)
        {
            return obj is Token token &&
                   Type == token.Type &&
                   Lexeme == token.Lexeme &&
                   EqualityComparer<object>.Default.Equals(Literal, token.Literal) &&
                   Line == token.Line &&
                   IsLiteral == token.IsLiteral;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() * 17 + 
                   Lexeme.GetHashCode() * 31 + 
                   Line.GetHashCode() * 43 + 
                   (Literal == null ? 0 : Literal.GetHashCode());
        }

        public static bool operator==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(Token left, Token right)
        {
            return !left.Equals(right);
        }
    }
}
