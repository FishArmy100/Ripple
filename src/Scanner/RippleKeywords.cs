using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    static class RippleKeywords
    {
        public static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
        {
            { "true", TokenType.True },
            { "false", TokenType.False },
            { "return", TokenType.Return },
            { "null", TokenType.Null },
            { "if", TokenType.If },
            { "Else", TokenType.Else },
            { "class", TokenType.Class },
            { "base", TokenType.Base },
            { "this", TokenType.This },
            { "while", TokenType.While },
            { "for", TokenType.For },
            { "public", TokenType.Public },
            { "private", TokenType.Private },
            { "protected", TokenType.Protected },
            { "friend", TokenType.Friend },
            { "new", TokenType.New },
            { "break", TokenType.Break },
            { "continue", TokenType.Continue },
            { "void", TokenType.Void },
            { "virutal", TokenType.Virtual },
            { "bool", TokenType.BoolType },
            { "char", TokenType.CharType },
            { "int", TokenType.IntType },
            { "uint", TokenType.UintType },
            { "float", TokenType.FloatType },
            { "string", TokenType.StringType },
            { "override", TokenType.Override},
            { "abstract", TokenType.Abstract },
            { "final", TokenType.Final },
            { "const", TokenType.Const }
        };

        public const string BOOL_TYPE_NAME = "bool";
        public const string CHAR_TYPE_NAME = "char";
        public const string INT_TYPE_NAME = "int";
        public const string FLOAT_TYPE_NAME = "float";
        public const string STRING_TYPE_NAME = "string";
    }
}
