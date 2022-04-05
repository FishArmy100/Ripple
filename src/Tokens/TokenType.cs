
namespace Ripple
{
    enum TokenType
    {
        // single charictor tokens
        Dot, Comma, Minus, Plus, Slash,
        Star, Semicolon, Colon, Caret, 
        Ampersand, Pipe, Percent, Tilda,
        QuestionMark,

        OpenParen, CloseParen, OpenBracket, CloseBracket,
        OpenBrace, CloseBrace,

        // One or tow character tokens
        Bang, BangEqual, Equal, EqualEqual, GreaterThen, GreaterThenEqual,
        LessThen, LessThenEqual, AmpersandAmpersand, PipePipe, LessThanLessThan, 
        GreaterThanGreaterThan,

        // Literals
        FloatLiteral, IntLiteral, UintLiteral, CharLiteral, CharArrayLiteral,

        // Keywords
        True, False, Return, Null, If, Else, Class,
        Base, This, While, For, Public, Private, 
        Protected, New, Break, Continue, Void, Virtual,
        Override, Static, 
        
        // Word operators
        DefaultOperator, SizeOfOperator, LengthOfOperator, ReinterpretCastOperator,

        // types
        BoolType, CharType, IntType, FloatType,

        Identifier,

        EndOfFile
    }

    static class TokenTypeExtentions
    {
        public static bool IsLiteral(this TokenType type)
        {
            return type == TokenType.IntLiteral ||
                   type == TokenType.UintLiteral ||
                   type == TokenType.FloatLiteral ||
                   type == TokenType.CharLiteral ||
                   type == TokenType.CharArrayLiteral;

        }

        public static bool IsBuiltInType(this TokenType type)
        {
            return type == TokenType.IntType   ||
                   type == TokenType.BoolType  ||
                   type == TokenType.FloatType ||
                   type == TokenType.CharType;

        }

        public static bool IsIdentifier(this TokenType type)
        {
            return type == TokenType.Identifier;
        }

        /// <summary>
        /// Checks if this token type is a built in type, identifier, or void
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTypeName(this TokenType type)
        {
            return type.IsBuiltInType() || type.IsIdentifier() || type == TokenType.Void;
        }
    }
}