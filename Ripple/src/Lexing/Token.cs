using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;

namespace Ripple.Lexing
{
    struct Token
    {
        public readonly string Text;
        public readonly TokenType Type;
        public readonly int Line;
        public readonly int Column;

        public Token(string text, TokenType type, int line, int column)
        {
            Text = text;
            Type = type;
            Line = line;
            Column = column;
        }

        public bool IsType(params TokenType[] tokenTypes)
        {
            return tokenTypes.Contains(Type);
        }

        public override string ToString()
        {
            return Text;
        }

        public string ToPrettyString()
        {
            if(Type.IsType(TokenType.Identifier, TokenType.CharactorLiteral, TokenType.StringLiteral, TokenType.CStringLiteral, TokenType.IntagerLiteral, TokenType.FloatLiteral))
                return "[" + Type.ToString() + ": " + Text + ": " + Line + ", " + Column + "]";
            else
                return "[" + Type.ToString() + ": " + Line + ", " + Column + "]";
        }

        public override bool Equals(object obj)
        {
            return obj is Token other &&
                other.Type == Type &&
                other.Text == Text;
        }

        public bool StrictEquals(Token token)
        {
            return base.Equals(token);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Text);
        }
    }
}
