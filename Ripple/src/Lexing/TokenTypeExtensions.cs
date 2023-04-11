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

        public static string ToPrettyString(this TokenType type)
        {
            return type switch
            {
                TokenType.OpenParen => "(",
                TokenType.CloseParen => ")",
                TokenType.OpenBrace => "{",
                TokenType.CloseBrace => "}",
                TokenType.OpenBracket => "[",
                TokenType.CloseBracket => "]",
                TokenType.Plus => "+",
                TokenType.Minus => "-",
                TokenType.Slash => "/",
                TokenType.Star => "*",
                TokenType.Mod => "%",
                TokenType.EqualEqual => "==",
                TokenType.Bang => "!",
                TokenType.BangEqual => "!=",
                TokenType.GreaterThanEqual => ">=",
                TokenType.GreaterThan => ">",
                TokenType.LessThanEqual => "<=",
                TokenType.LessThan => "<",
                TokenType.AmpersandAmpersand => "&&",
                TokenType.PipePipe => "||",
                TokenType.Ampersand => "&",
                TokenType.SemiColon => ";",
                TokenType.Equal => "=",
                TokenType.RightThinArrow => "->",
                TokenType.Identifier => "Identifier",
                TokenType.Comma => ",",
                TokenType.Lifetime => "Lifetime",
                TokenType.RefMut => "&mut",
                TokenType.IntagerLiteral => "Intager Literal",
                TokenType.FloatLiteral => "Float Literal",
                TokenType.CharactorLiteral => "Charactor Literal",
                TokenType.StringLiteral => "String Literal",
                TokenType.Nullptr => "nullptr",
                TokenType.CStringLiteral => "C String Literal",
                TokenType.Func => "func",
                TokenType.For => "for",
                TokenType.If => "if",
                TokenType.True => "true",
                TokenType.False => "false",
                TokenType.Return => "return",
                TokenType.Break => "break",
                TokenType.Continue => "continue",
                TokenType.While => "while",
                TokenType.Else => "else",
                TokenType.As => "as",
                TokenType.Mut => "mut",
                TokenType.Extern => "extern",
                TokenType.Where => "where",
                TokenType.Class => "class",
                TokenType.Public => "public",
                TokenType.Private => "private",
                TokenType.Unsafe => "unsafe",
                TokenType.Sizeof => "sizeof",
                TokenType.EOF => "End of File",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
