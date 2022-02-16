
namespace Ripple
{
    enum TokenType
    {
        Invalid,

        // single charictor tokens
        Dot, Comma, Minus, Plus, Slash, 
        Star, Semicolon, Carrot, Ampersand,

        OpenParen, CloseParen, OpenBracket, CloseBracket,
        OpenBrace, CloseBrace,

        // One or tow character tokens
        Bang, BangEqual, Equal, EqualEqual, GreaterThen, GreaterThenEqual,
        LessThen, LessThenEqual, 

        // Literals
        Float, Int, Uint, Char, String,

        // Keywords
        And, Or, Xor, True, False, Return, Null, If, Else, Class,
        Extends, Base, This, While, For, Public, Private, 
        Protected, Friend, New, Break, Continue, Void, Virtual,
        Override,

        // type names
        BoolType, CharType, IntType, UintType, FloatType, StringType,

        Identifier,

        EOF
    }
}