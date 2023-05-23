using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing
{
    public enum TokenType
    {
        OpenParen, CloseParen, OpenBrace, CloseBrace,
        OpenBracket, CloseBracket,

        // Arithmatic
        Plus, Minus, Slash, Star, Mod,

        // Logical
        EqualEqual, Bang, BangEqual, GreaterThanEqual, 
        GreaterThan, LessThanEqual, LessThan, 
        AmpersandAmpersand, PipePipe, Ampersand,

        // Misc
        SemiColon, Equal, RightThinArrow, Identifier, Comma, Lifetime,
        RefMut, Tilda, Dot,

        // Literals
        IntagerLiteral, FloatLiteral, CharactorLiteral, StringLiteral,
        Nullptr, CStringLiteral,

        // Keywords
        Func, For, If, True, False, Return, 
        Break, Continue, While, Else, As, 
        Mut, Extern, This,

        Where, Class, Public, Private, Unsafe, Sizeof,

        EOF,
    }
}
