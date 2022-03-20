
namespace Ripple
{
    enum TokenType
    {
        Invalid,

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
        FloatLiteral, IntLiteral, UintLiteral, CharLiteral, StringLiteral,

        // Keywords
        True, False, Return, Null, If, Else, Class,
        Base, This, While, For, Public, Private, 
        Protected, Friend, New, Break, Continue, Void, Virtual,
        Override, Abstract, Final, Const, Static,

        // types
        BoolType, CharType, IntType, UintType, FloatType, StringType, ObjectType,

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
                   type == TokenType.StringLiteral;

        }

        public static bool IsBuiltInType(this TokenType type)
        {
            return type == TokenType.IntType    ||
                   type == TokenType.UintType   ||
                   type == TokenType.BoolType   ||
                   type == TokenType.FloatType  ||
                   type == TokenType.CharType   ||
                   type == TokenType.StringType ||
                   type == TokenType.ObjectType;

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