using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;
using Raucse.Extensions;
using Ripple.Core;

namespace Ripple.Lexing
{
    class TokenBuilder
    {
        private StringReader m_Reader;
        private string m_FilePath;
        private int m_FirstCharOfToken = 0;

        public void SetSource(string source, string file)
        {
            m_Reader = new StringReader(source);
            m_FilePath = file;
            m_FirstCharOfToken = 0;
        }

        public Option<Result<Token, LexerError>> Next()
        {
            if (m_Reader == null)
                return new Option<Result<Token, LexerError>>();

            ClearCommentsAndWhitespace();

            if (m_Reader.IsAtEnd())
                return new Option<Result<Token, LexerError>>();

            var symbol = LexSymbol();
            if (symbol.HasValue())
                return symbol.Value;

            var strLit = LexStringLiteral();
            if (strLit.HasValue())
                return strLit;

            var numLit = LexNumberLiteral();
            if (numLit.HasValue())
                return numLit;

            var id = LexIdentifierOrKeyword();
            if (id.HasValue())
                return id;

            var charLit = LexCharactorLiteralOrLifetime();
            if (charLit.HasValue())
                return charLit;

            string unknown = m_Reader.Advance().ToString();
            return CreateError($"Unknown charactor '{unknown}'");
        }

        public bool IsAtEnd() => m_Reader.IsAtEnd();

        private Option<Result<Token, LexerError>> LexNumberLiteral()
        {
            if (m_Reader.Current().IsDigit())
            {
                // lexing int
                string src = m_Reader.Advance().ToString();
                while (!m_Reader.IsAtEnd() && m_Reader.Current().IsDigit())
                    src += m_Reader.Advance();

                // lexing float
                if (!m_Reader.IsAtEnd() && m_Reader.Current() == '.')
                {
                    char? nc = m_Reader.Peek();
                    if (nc.HasValue && nc.Value.IsDigit())
                    {
                        src += m_Reader.Advance().ToString();
                        src += m_Reader.Advance().ToString();

                        while (!m_Reader.IsAtEnd() && m_Reader.Current().IsDigit())
                            src += m_Reader.Advance();

                        return CreateTokenResult(TokenType.FloatLiteral, src);
                    }
                    else
                    {
                        m_Reader.Advance();
                        return CreateError("Float literal must have a digit after the decimal point. you can use .0");
                    }
                }

                return CreateTokenResult(TokenType.IntagerLiteral, src);
            }

            return new Option<Result<Token, LexerError>>();
        }

        private Option<Result<Token, LexerError>> LexStringLiteral()
        {
            Option<char> prefix = m_Reader.CheckSequence('c', '\"') ?
                    m_Reader.Advance() :
                    new Option<char>();

            if (m_Reader.Current() == '\"')
            {
                string text = m_Reader.Advance().ToString();
                while (!m_Reader.IsAtEnd() && m_Reader.Current() != '\"')
                {
                    if(m_Reader.Current() == '\\')
                    {
                        text += m_Reader.Advance();
                        if (!m_Reader.IsAtEnd())
                            text += m_Reader.Advance();
                        else
                            return CreateError("Expected another charactor.");
                    }
                    else
                    {
                        text += m_Reader.Advance();
                    }
                }

                if (m_Reader.IsAtEnd())
                    return CreateError("Unterminated string.");

                text += m_Reader.Advance();

                return prefix.Match(
                    ok => CreateTokenResult(TokenType.CStringLiteral, prefix + text),
                    () => CreateTokenResult(TokenType.StringLiteral, text));
            }

            return new Option<Result<Token, LexerError>>();
        }

        public Option<Result<Token, LexerError>> LexIdentifierOrKeyword()
        {
            if(m_Reader.Current().IsAlpha())
            {
                string text = m_Reader.Advance().ToString();
                while (!m_Reader.IsAtEnd() && m_Reader.Current().IsAlphaNumeric())
                    text += m_Reader.Advance();

                if (RippleKeywords.Keywords.TryGetValue(text, out TokenType type))
                    return CreateTokenResult(type, text);

                return CreateTokenResult(TokenType.Identifier, text);
            }

            return new Option<Result<Token, LexerError>>();
        }

        private Option<Result<Token, LexerError>> LexCharactorLiteralOrLifetime()
        {
            if (m_Reader.Current() == '\'')
            {
                string text = m_Reader.Advance().ToString();
                if (m_Reader.IsAtEnd())
                    return CreateError("Expected a charactor.");

                char c = m_Reader.Advance();
                if (c == '\'')
                    return CreateError("Expected a charactor.");

                text += c;

                if(c == '\\')
                {
                    if (m_Reader.IsAtEnd())
                        return CreateError("Expected a charactor.");
                    char special = m_Reader.Advance();
                    text += special;
                }

                if (m_Reader.Match('\''))
                {
                    text += m_Reader.Previous();
                    return CreateTokenResult(TokenType.CharactorLiteral, text);
                }

                if (!text[1].IsAlpha()) // ie: '1
                    return CreateError("Lifetime must start with an alpha charactor");

                while (!m_Reader.IsAtEnd() && m_Reader.Current().IsAlphaNumeric())
                    text += m_Reader.Advance();

                return CreateTokenResult(TokenType.Lifetime, text);
            }

            return new Option<Result<Token, LexerError>>();
        }

        private Option<Result<Token, LexerError>> LexSymbol()
        {
            Option<Result<Token, LexerError>> GenToken(TokenType type, int length)
            {
                string src = "";
                int line = m_Reader.Line;
                int col = m_Reader.Column;

                for (int i = 0; i < length; i++)
                    src += m_Reader.Advance();
                return new Option<Result<Token, LexerError>>(CreateToken(type, src));
            }

            char c = m_Reader.Current();
            char? nc = m_Reader.Peek();

            switch (c)
            {
                // single
                case '+':
                    return GenToken(TokenType.Plus, 1);
                case '/':
                    return GenToken(TokenType.Slash, 1);
                case '*':
                    return GenToken(TokenType.Star, 1);
                case '%':
                    return GenToken(TokenType.Mod, 1);
                case ';':
                    return GenToken(TokenType.SemiColon, 1);
                case ',':
                    return GenToken(TokenType.Comma, 1);
                case '(':
                    return GenToken(TokenType.OpenParen, 1);
                case ')':
                    return GenToken(TokenType.CloseParen, 1);
                case '{':
                    return GenToken(TokenType.OpenBrace, 1);
                case '}':
                    return GenToken(TokenType.CloseBrace, 1);
                case '[':
                    return GenToken(TokenType.OpenBracket, 1);
                case ']':
                    return GenToken(TokenType.CloseBracket, 1);

                // multiple
                case '=':
                    if (nc.HasValue && nc.Value == '=')
                    {
                        return GenToken(TokenType.EqualEqual, 2);
                    }
                    return GenToken(TokenType.Equal, 1);

                case '<':
                    if (nc.HasValue && nc.Value == '=')
                    {
                        return GenToken(TokenType.LessThanEqual, 2);
                    }
                    return GenToken(TokenType.LessThan, 1);

                case '>':
                    if (nc.HasValue && nc.Value == '=')
                    {
                        return GenToken(TokenType.GreaterThanEqual, 2);
                    }
                    return GenToken(TokenType.GreaterThan, 1);

                case '!':
                    if (nc.HasValue && nc.Value == '=')
                    {
                        return GenToken(TokenType.BangEqual, 2);
                    }
                    return GenToken(TokenType.Bang, 1);
                case '-':
                    if (nc.HasValue && nc.Value == '>')
                    {
                        return GenToken(TokenType.RightThinArrow, 2);
                    }
                    return GenToken(TokenType.Minus, 1);

                case '&':
                    return GenToken(TokenType.Ampersand, 1);

                case '|':
                    if (nc.HasValue && nc.Value == '|')
                    {
                        return GenToken(TokenType.PipePipe, 2);
                    }
                    break;
                default:
                    break;
            }

            return new Option<Result<Token, LexerError>>();
        }

        private void ClearCommentsAndWhitespace()
        {
            while (!m_Reader.IsAtEnd() && (ClearComments() || ClearWhitespace()))
                continue;

            m_FirstCharOfToken = m_Reader.Index;
        }

        private bool ClearWhitespace()
        {
            if(m_Reader.Current().IsWhiteSpace())
            {
                while (!m_Reader.IsAtEnd() && m_Reader.Current().IsWhiteSpace())
                    m_Reader.Advance();

                return true;
            }

            return false;
        }

        private bool ClearComments()
        {
            char c = m_Reader.Current();
            char? nc = m_Reader.Peek();

            if (nc == null)
                return false;

            if (c == '/' && nc.Value == '/')
            {
                while (!m_Reader.IsAtEnd() && m_Reader.Current() != '\n')
                    m_Reader.Advance();

                return true;
            }
            else if (c == '/' && nc.Value == '*')
            {
                m_Reader.Advance();
                m_Reader.Advance();

                while (!m_Reader.IsAtEnd())
                {
                    c = m_Reader.Advance();
                    nc = m_Reader.Peek();

                    if (nc.HasValue && c == '*' && nc.Value == '/')
                        break;
                }

                m_Reader.Advance();
                m_Reader.Advance();
                return true;
            }

            return false;
        }

        private Token CreateToken(TokenType type, Option<object> value)
        {
            SourceLocation location = new SourceLocation(m_FirstCharOfToken, m_Reader.Index, m_FilePath);
            bool hasSpaceAfter = !m_Reader.IsAtEnd() && m_Reader.Current().IsWhiteSpace();
            Token token = new Token(value, location, type, hasSpaceAfter);
            m_FirstCharOfToken = m_Reader.Index;
            return token;
        }

        private Option<Result<Token, LexerError>> CreateTokenResult(TokenType type, string text)
        {
            return new Option<Result<Token, LexerError>>(new Result<Token, LexerError>(CreateToken(type, text)));
        }

        private Option<Result<Token, LexerError>> CreateError(string errorText)
        {
            return new Option<Result<Token, LexerError>>(new Result<Token, LexerError>(new LexerError(errorText, m_Reader.Line, m_Reader.Column)));
        }
    }
}
