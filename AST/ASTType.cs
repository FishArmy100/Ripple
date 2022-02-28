using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct ASTType
    {
        public readonly TokenType Type;
        public readonly string Identifier;

        public bool IsIdentifier => Identifier != null && Type == TokenType.Identifier;
        public bool IsNull => Type == TokenType.Null;

        public ASTType(string identifier)
        {
            Type = TokenType.Identifier;
            Identifier = identifier;
        }

        public ASTType(TokenType type)
        {
            Type = type;
            Identifier = null;
        }

        public ASTType(Token token)
        {
            if(token.Type == TokenType.Identifier)
            {
                this = new ASTType(token.Lexeme);
            }

            this = new ASTType(token.Type);
        }

        public override bool Equals(object obj)
        {
            return obj is ASTType type &&
                   Type == type.Type &&
                   Identifier == type.Identifier &&
                   IsIdentifier == type.IsIdentifier;
        }

        public static bool operator==(ASTType left, ASTType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ASTType left, ASTType right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            if (IsIdentifier)
                return Identifier.GetHashCode();

            return Type.GetHashCode();
        }
    }
}
