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

        // Arithmatic
        Plus, Minus, Slash, Star, Mod,

        // Logical
        EqualEqual, Bang, BangEqual, GreaterThanEqual, 
        GreaterThan, LessThanEqual, LessThan, 
        AmpersandAmpersand, PipePipe,

        // Misc
        SemiColon, Equal, RightThinArrow, Identifier, Comma,

        // Literals
        IntagerLiteral, FloatLiteral,

        // Keywords
        Func, For, If, True, False, Return,

        Unknown, EOF,
    }
}
