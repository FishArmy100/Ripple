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
        SemiColon, Equal, RightThinArrow, Identifier, Comma, Lifetime,
        RefMut,

        // Literals
        IntagerLiteral, FloatLiteral, CharactorLiteral, StringLiteral,
        Nullptr,

        // Keywords
        Func, For, If, True, False, Return, 
        Break, Continue, While, Else, As, 
        Mut, Extern, 
        
        Where, Class, Public, Private, Unsafe, Sizeof,

        Unknown, EOF,
    }
}
