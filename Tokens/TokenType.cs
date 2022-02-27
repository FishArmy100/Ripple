
namespace Ripple
{
    enum TokenType
    {
        Invalid,

        // single charictor tokens
        Dot, Comma, Minus, Plus, Slash,
        Star, Semicolon, Colon, Caret, 
        Ampersand, Pipe, Percent,

        OpenParen, CloseParen, OpenBracket, CloseBracket,
        OpenBrace, CloseBrace,

        // One or tow character tokens
        Bang, BangEqual, Equal, EqualEqual, GreaterThen, GreaterThenEqual,
        LessThen, LessThenEqual, AmpersandAmpersand, PipePipe, LessThanLessThan, 
        GreaterThanGreaterThan,

        // Literals
        Float, Int, Uint, Char, String,

        // Keywords
        True, False, Return, Null, If, Else, Class,
        Base, This, While, For, Public, Private, 
        Protected, Friend, New, Break, Continue, Void, Virtual,
        Override, Abstract, Final, Const,

        // types
        BoolType, CharType, IntType, UintType, FloatType, StringType, ObjectType,

        Identifier,

        EndOfFile
    }
}