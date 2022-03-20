using System;
using System.Collections.Generic;
using System.Text;
using Ripple.Utils;

namespace Ripple
{
    static class Scanner
    {
        private static List<Token> Tokens;
        private static List<ScannerError> Errors;
        private static SourceReader Reader;

        public static OperationResult<List<Token>, ScannerError> GetTokens(string source)
        {
            Reset(source);
            while(!Reader.IsAtEnd())
            {
                Reader.UpdateStart();
                if (ScanEmpty())
                    continue;
                else if (ScanSymbol())
                    continue;
                else if (ScanChar())
                    continue;
                else if (ScanString())
                    continue;
                else if (ScanNumber())
                    continue;
                else if (ScanKeyword())
                    continue;
                else
                {
                    AddError("Invalid symbole: \"" + Reader.PeekCurrent() + "\"");
                    Reader.AdvanceCurrent();
                }
            }

            Tokens.Add(new Token(TokenType.EndOfFile, "", Reader.Line, Reader.Column));
            return new OperationResult<List<Token>, ScannerError>(Tokens, Errors);
        }

        private static bool ScanEmpty()
        {
            char c = Reader.PeekCurrent();
            if(c == ' ' || c == '\n' || c == '\t')
            {
                Reader.AdvanceCurrent();
                return true;
            }

            return false;
        }

        private static bool ScanKeyword()
        {
            if(IsAlpha(Reader.PeekCurrent()))
            {
                while (IsAlphaOrDigit(Reader.PeekCurrent()))
                    Reader.AdvanceCurrent();

                if(RippleKeywords.Keywords.TryGetValue(Reader.GetStartToCurrentString(), out TokenType type))
                {
                    switch (type)
                    {
                        case TokenType.True:
                            AddToken(type, true);
                            break;
                        case TokenType.False:
                            AddToken(type, false);
                            break;
                        case TokenType.Null:
                            AddToken(type, null);
                            break;
                        default:
                            AddToken(type);
                            break;
                    }
                }
                else
                    AddToken(TokenType.Identifier, Reader.GetStartToCurrentString());

                return true;
            }

            return false;
        }

        private static bool ScanString()
        {
            if(Reader.PeekCurrent() == '\"')
            {
                Reader.AdvanceCurrent();

                while (Reader.PeekCurrent() != '\"' && !Reader.IsAtEnd())
                    Reader.AdvanceCurrent();

                Reader.AdvanceCurrent(); // moves past token

                if (Reader.IsAtEnd() && Reader.PeekCurrent(-1) != '\"')
                    AddError("Untermenated string");

                AddToken(TokenType.StringLiteral, Reader.Source[(Reader.Start + 1)..(Reader.Current - 1)]);
                return true;
            }

            return false;
        }


        private static bool ScanChar()
        {
            if(Reader.PeekCurrent() == '\'')
            {
                Reader.AdvanceCurrent();

                while (Reader.PeekCurrent() != '\'' && !Reader.IsAtEnd())
                    Reader.AdvanceCurrent();

                Reader.AdvanceCurrent(); // moves past token

                if(Reader.Current - Reader.Start != 3)
                {
                    AddError("Charactor literal cant be more than one charactor");
                    return true;
                }

                if (Reader.IsAtEnd() && Reader.PeekCurrent(-1) != '\'')
                    AddError("Untermenated char");

                AddToken(TokenType.CharLiteral, Reader.Source[(Reader.Start + 1)..(Reader.Current - 1)]);
                return true;
            }

            return false;
        }

        private static bool ScanNumber()
        {
            if (!IsDigit(Reader.PeekCurrent()))
                return false;

            if (Reader.PeekCurrent() == '.')
                return false;

            int dotCount = 0;

            while (IsDigit(Reader.PeekCurrent()) || Reader.PeekCurrent() == '.')
            {
                if (Reader.PeekCurrent() == '.')
                    dotCount++;

                Reader.AdvanceCurrent();
            }

            if(Reader.PeekCurrent(-1) == '.')
            {
                AddError("Cant Have Trailing decimal point");
                return true;
            }


            if(dotCount > 1)
            {
                AddError("To many decimal points in float literal");
                return true;
            }

            if (dotCount == 1)
                AddToken(TokenType.FloatLiteral, float.Parse(Reader.GetStartToCurrentString()));
            if(dotCount == 0)
                AddToken(TokenType.IntLiteral, int.Parse(Reader.GetStartToCurrentString()));

            return true;
        }

        private static bool ScanSymbol()
        {
            char c = Reader.PeekCurrent();
            bool foundSymbol = false;
            bool isComment = false;

            void FoundToken(TokenType type, int size = 1)
            {
                Reader.AdvanceCurrent(size);
                AddToken(type);
                foundSymbol = true;
            }

            switch(c)
            {
                // single charactor symbols
                case '+':
                    FoundToken(TokenType.Plus);
                    break;
                case '-':
                    FoundToken(TokenType.Minus);
                    break;
                case '*':
                    FoundToken(TokenType.Star);
                    break;
                case '%':
                    FoundToken(TokenType.Percent);
                    break;
                case '^':
                    FoundToken(TokenType.Caret);
                    break;
                case '.':
                    FoundToken(TokenType.Dot);
                    break;
                case ',':
                    FoundToken(TokenType.Comma);
                    break;
                case '(':
                    FoundToken(TokenType.OpenParen);
                    break;
                case ')':
                    FoundToken(TokenType.CloseParen);
                    break;
                case '{':
                    FoundToken(TokenType.OpenBrace);
                    break;
                case '}':
                    FoundToken(TokenType.CloseBrace);
                    break;
                case '[':
                    FoundToken(TokenType.OpenBracket);
                    break;
                case ']':
                    FoundToken(TokenType.CloseBracket);
                    break;
                case ';':
                    FoundToken(TokenType.Semicolon);
                    break;
                case ':':
                    FoundToken(TokenType.Colon);
                    break;
                case '~':
                    FoundToken(TokenType.Tilda);
                    break;
                case '?':
                    FoundToken(TokenType.QuestionMark);
                    break;

                // multiple charictor symbols
                case '!':
                    if (Reader.PeekCurrent(1) == '=')
                        FoundToken(TokenType.BangEqual, 2);
                    else
                        FoundToken(TokenType.Bang);
                    foundSymbol = true;
                    break;
                case '=':
                    if (Reader.PeekCurrent(1) == '=')
                        FoundToken(TokenType.EqualEqual, 2);
                    else
                        FoundToken(TokenType.Equal);
                    foundSymbol = true;
                    break;
                case '<':
                    if (Reader.PeekCurrent(1) == '=')
                        FoundToken(TokenType.LessThenEqual, 2);
                    else if (Reader.PeekCurrent(1) == '<')
                        FoundToken(TokenType.LessThanLessThan, 2);
                    else
                        FoundToken(TokenType.LessThen);
                    foundSymbol = true;
                    break;
                case '>':
                    if (Reader.PeekCurrent(1) == '=')
                        FoundToken(TokenType.GreaterThenEqual, 2);
                    else if (Reader.PeekCurrent(1) == '>')
                        FoundToken(TokenType.GreaterThanGreaterThan, 2);
                    else
                        FoundToken(TokenType.GreaterThen);
                    foundSymbol = true;
                    break;
                case '&':
                    if (Reader.PeekCurrent(1) == '&')
                        FoundToken(TokenType.AmpersandAmpersand, 2);
                    else 
                        FoundToken(TokenType.Ampersand);
                    foundSymbol = true;
                    break;
                case '|':
                    if (Reader.PeekCurrent(1) == '|')
                        FoundToken(TokenType.PipePipe, 2);
                    else
                        FoundToken(TokenType.Pipe);
                    foundSymbol = true;
                    break;

                // Comments and /
                case '/':
                    if (Reader.PeekCurrent(1) == '/')
                    {
                        while (Reader.AdvanceCurrent() != '\n' && !Reader.IsAtEnd())
                            continue;
                        isComment = true;
                    }
                    else
                    {
                        FoundToken(TokenType.Slash);
                    }
                    break;

                default:
                    break;
       
            }

            if(foundSymbol && !isComment)
            {
                return true;
            }

            if(isComment)
                return true;

            return false;
        }

        private static void AddError(string message, string where = "")
        {
            Errors.Add(new ScannerError(message, Reader.Line, Reader.Column, where));
        }

        private static void AddToken(TokenType type, object literal = null)
        {
            Tokens.Add(new Token(type, Reader.GetStartToCurrentString(), literal, Reader.Line, Reader.Column));
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private static bool IsAlphaOrDigit(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private static void Reset(string source)
        {
            Tokens = new List<Token>();
            Errors = new List<ScannerError>();
            Reader = new SourceReader(source);
        }
    }
}
