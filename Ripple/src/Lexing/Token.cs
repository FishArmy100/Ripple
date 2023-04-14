using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse.Extensions;
using Raucse;
using Ripple.Core;

namespace Ripple.Lexing
{
    public struct Token
    {
        public readonly Option<object> Value;
        public readonly SourceLocation Location;
        public readonly TokenType Type;
        public readonly bool HasSpaceAfter;

        public Token(Option<object> value, SourceLocation location, TokenType type, bool hasSpaceAfter)
        {
            Value = value;
            Location = location;
            Type = type;
            HasSpaceAfter = hasSpaceAfter;
        }

        public string Text
        {
            get
            {
                if(Type.IsType(TokenType.Identifier, TokenType.Lifetime, 
                               TokenType.IntagerLiteral, TokenType.FloatLiteral, 
                               TokenType.CharactorLiteral, TokenType.CStringLiteral, 
                               TokenType.EOF, TokenType.StringLiteral))
                {
                    return Value.Value.ToString();
                }

                return Type.ToPrettyString();
            }
        }

        public bool IsType(params TokenType[] tokenTypes)
        {
            return tokenTypes.Contains(Type);
        }

        public override string ToString()
        {
            if (Type.IsType(TokenType.Identifier, TokenType.Lifetime,
                            TokenType.IntagerLiteral, TokenType.FloatLiteral,
                            TokenType.CharactorLiteral, TokenType.CStringLiteral,
                            TokenType.EOF, TokenType.StringLiteral))
            {
                return $"[{Type}{Value.Match(ok => ": " + ok, () => "")}]";
            }
            else
            {
                return $"[{Type}]";
            }
        }

        public bool StrictEquals(Token token)
        {
            return base.Equals(token);
        }

        public override bool Equals(object obj)
        {
            return obj is Token token &&
                   EqualityComparer<Option<object>>.Default.Equals(Value, token.Value) &&
                   Type == token.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Type);
        }

        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }
    }
}
