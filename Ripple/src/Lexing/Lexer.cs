using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.Lexing
{
    static class Lexer
    {
        public static Result<List<Token>, List<LexerError>> Scan(string src)
        {
            StringReader reader = new StringReader(src);
            List<Token> tokens = new List<Token>();
            List<LexerError> errors = new List<LexerError>();
            
            while(!reader.IsAtEnd())
            {
                if (ScanComments(ref reader))
                    continue;

                if(reader.Current().IsWhiteSpace())
                {
                    reader.Advance();
                    continue;
                }

                if (ScanIdentifierOrKeyword(ref reader, out Token tok))
                {
                    tokens.Add(tok);
                }
                else if (ScanSymbol(ref reader, out tok))
                {
                    tokens.Add(tok);
                }
                else if(ScanNumberLiteral(ref reader, ref errors, out tok))
                {
                    tokens.Add(tok);
                }
                else
                {
                    errors.Add(new LexerError("Unknown token: " + reader.Current(), reader.Line, reader.Column));
                    reader.Advance();
                }
            }

            tokens.Add(new Token("", TokenType.EOF, reader.Line, reader.Line));

            if (errors.Count > 0)
                return new Result<List<Token>, List<LexerError>>.Fail(errors);

            return new Result<List<Token>, List<LexerError>>.Ok(tokens);
        }

        private static bool ScanNumberLiteral(ref StringReader reader, ref List<LexerError> errors, out Token tok)
        {
            if(reader.Current().IsNumeric())
            {
                int line = reader.Line;
                int col = reader.Column;

                // lexing int
                string src = reader.Advance().ToString();
                while (!reader.IsAtEnd() && reader.Current().IsNumeric())
                    src += reader.Advance();

                // lexing float
                if(!reader.IsAtEnd() && reader.Current() == '.')
                {
                    char? nc = reader.Peek();
                    if(nc.HasValue && nc.Value.IsNumeric())
                    {
                        src += reader.Advance().ToString(); 
                        src += reader.Advance().ToString();

                        while (!reader.IsAtEnd() && reader.Current().IsNumeric())
                            src += reader.Advance();

                        tok = new Token(src, TokenType.FloatLiteral, line, col);
                        return true;
                    }
                    else
                    {
                        reader.Advance();
                        errors.Add(new LexerError("Float literal must have a digit after the decimal point. you can use .0", reader.Line, reader.Column));
                        tok = new Token();
                        return false;
                    }
                }

                tok = new Token(src, TokenType.IntagerLiteral, line, col);
                return true;
            }

            tok = new Token();
            return false;
        }

        private static bool ScanIdentifierOrKeyword(ref StringReader reader, out Token tok)
        {
            if(reader.Current().IsAlpha())
            {
                int line = reader.Line;
                int col = reader.Column;

                string id = reader.Advance().ToString();
                while(!reader.IsAtEnd() && reader.Current().IsAlphaNumeric())
                {
                    id += reader.Advance();
                }

                if (RippleKeywords.Keywords.TryGetValue(id, out TokenType t))
                    tok = new Token(id, t, line, col);
                else
                    tok = new Token(id, TokenType.Identifier, line, col);

                return true;
            }

            tok = new Token();
            return false;
        }

        private static bool ScanSymbol(ref StringReader reader, out Token tok)
        {
            Token GenToken(ref StringReader reader, TokenType type, int length)
            {
                string src = "";
                int line = reader.Line;
                int col = reader.Column;

                for (int i = 0; i < length; i++)
                    src += reader.Advance();
                return new Token(src, type, line, col);
            }

            char c = reader.Current();
            char? nc = reader.Peek();

            switch (c)
            {
                // single
                case '+':
                    tok = GenToken(ref reader, TokenType.Plus, 1);
                    return true;
                case '/':
                    tok = GenToken(ref reader, TokenType.Slash, 1);
                    return true;
                case '*':
                    tok = GenToken(ref reader, TokenType.Star, 1);
                    return true;
                case '%':
                    tok = GenToken(ref reader, TokenType.Mod, 1);
                    return true;
                case ';':
                    tok = GenToken(ref reader, TokenType.SemiColon, 1);
                    return true;
                case ',':
                    tok = GenToken(ref reader, TokenType.Comma, 1);
                    return true;
                case '(':
                    tok = GenToken(ref reader, TokenType.OpenParen, 1);
                    return true;
                case ')':
                    tok = GenToken(ref reader, TokenType.CloseParen, 1);
                    return true;
                case '{':
                    tok = GenToken(ref reader, TokenType.OpenBrace, 1);
                    return true;
                case '}':
                    tok = GenToken(ref reader, TokenType.CloseBrace, 1);
                    return true;

                // multiple
                case '=':
                    if(nc.HasValue)
                    {
                        if(nc.Value == '=')
                        {
                            tok = GenToken(ref reader, TokenType.EqualEqual, 2);
                            return true;
                        }
                    }
                    tok = GenToken(ref reader, TokenType.Equal, 1);
                    return true;

                case '<':
                    if (nc.HasValue)
                    {
                        if (nc.Value == '=')
                        {
                            tok = GenToken(ref reader, TokenType.LessThanEqual, 2);
                            return true;
                        }
                    }
                    tok = GenToken(ref reader, TokenType.LessThan, 1);
                    return true;

                case '>':
                    if (nc.HasValue)
                    {
                        if (nc.Value == '=')
                        {
                            tok = GenToken(ref reader, TokenType.GreaterThanEqual, 2);
                            return true;
                        }
                    }
                    tok = GenToken(ref reader, TokenType.GreaterThan, 1);
                    return true;

                case '!':
                    if (nc.HasValue)
                    {
                        if (nc.Value == '=')
                        {
                            tok = GenToken(ref reader, TokenType.BangEqual, 2);
                            return true;
                        }
                    }
                    tok = GenToken(ref reader, TokenType.Bang, 1);
                    return true;

                case '-':
                    if (nc.HasValue)
                    {
                        if (nc.Value == '>')
                        {
                            tok = GenToken(ref reader, TokenType.RightThinArrow, 2);
                            return true;
                        }
                    }
                    tok = GenToken(ref reader, TokenType.Minus, 1);
                    return true;

                case '&':
                    if (nc.HasValue)
                    {
                        if (nc.Value == '&')
                        {
                            tok = GenToken(ref reader, TokenType.AmpersandAmpersand, 2);
                            return true;
                        }
                    }
                    break;

                case '|':
                    if (nc.HasValue)
                    {
                        if (nc.Value == '|')
                        {
                            tok = GenToken(ref reader, TokenType.PipePipe, 2);
                            return true;
                        }
                    }
                    break;

                default:
                    break;
            }

            tok = new Token();
            return false;
        }

        private static bool ScanComments(ref StringReader reader)
        {
            char c = reader.Current();
            char? nc = reader.Peek();

            if (nc == null)
                return false;

            if(c == '/' && nc.Value == '/')
            {
                while (reader.Advance() != '\n' && !reader.IsAtEnd())
                    continue;
            }
            else if(c == '/' && nc.Value == '*')
            {
                while(!reader.IsAtEnd())
                {
                    c = reader.Advance();
                    nc = reader.Peek();

                    if (nc.HasValue && c == '*' && nc.Value == '/')
                        break;
                }

                reader.Advance();
                reader.Advance();
                return true;
            }

            return false;
        }
    }
}
