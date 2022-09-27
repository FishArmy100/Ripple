using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Lexing
{
    enum TokenType
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
        SemiColon, Equal, RightThinArrow, Identifier, Comma,

        // Literals
        IntagerLiteral, FloatLiteral, CharactorLiteral, StringLiteral,

        // Keywords
        Func, For, If, True, False, Return, 
        Ref, Break, Continue, While, Else, 
        As, Mut, Extern,

        Unknown, EOF,
    }
}
