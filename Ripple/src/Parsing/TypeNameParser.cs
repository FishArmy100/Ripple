using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;
using Raucse;
using Ripple.Utils;
using Ripple.Parsing.Errors;

namespace Ripple.Parsing
{
    static class TypeNameParser
    {
        public static bool IsTypeName(ref TokenReader reader, out int length, int beginOffset  = 0)
        {
            length = 0;
            TokenReader typeReader = new TokenReader(reader.GetTokens(), reader.Index + beginOffset);
            try
            {
                int initialIndex = typeReader.Index;
                _ = ParseTypeName(ref typeReader);
                int newIndex = typeReader.Index;
                length = newIndex - initialIndex;
                return true;
            }
            catch(ReaderAtEndExeption)
            {
                return false;
            }
            catch(ParserExeption)
            {
                return false;
            }
        }

        public static TypeName ParseTypeName(ref TokenReader reader)
        {
            if (reader.Current().Type.IsIdentifier())
                return ParseSimpleTypeName(ref reader);
            else
                return ParseComplexType(ref reader);
        }

        private static TypeName ParseSimpleTypeName(ref TokenReader reader)
        {
            Token identifier = reader.Consume(TokenType.Identifier);
            TypeName type = new BasicType(identifier);
            return ParseTypePrefixRecursive(ref reader, type);
        }

        private static TypeName ParseTypePrefixRecursive(ref TokenReader reader, TypeName previous)
        {
            Token? mut = null;
            if (reader.CheckSequence(TokenType.Mut, TokenType.Ampersand) || reader.CheckSequence(TokenType.Mut, TokenType.Star))
                mut = reader.Advance();

            if(reader.Match(TokenType.Star))
            {
                TypeName ptrType = new PointerType(previous, mut, reader.Previous());
                return ParseTypePrefixRecursive(ref reader, ptrType);
            }
            else if(reader.Match(TokenType.Ampersand))
            {
                Token anperand = reader.Previous();
                Token? lifetime = reader.TryMatch(TokenType.Lifetime);
                TypeName refType = new ReferenceType(previous, mut, anperand, lifetime);
                return ParseTypePrefixRecursive(ref reader, refType);
            }
            else if(reader.Match(TokenType.OpenBracket))
            {
                if (mut.HasValue)
                    throw new ParserExeption(new ExpectedTokenError(reader.Previous().Location, TokenType.Ampersand, TokenType.Star));

                Token openBracket = reader.Previous();
                Token size = reader.Consume(TokenType.IntagerLiteral);
                Token closeBracket = reader.Consume(TokenType.CloseBracket);
                TypeName arrType = new ArrayType(previous, openBracket, size, closeBracket);
                return ParseTypePrefixRecursive(ref reader, arrType);
            }
            else
            {
                return previous;
            }
        }

        private static TypeName ParseComplexType(ref TokenReader reader)
        {
            if(reader.Current().Type == TokenType.OpenParen)
            {
                TypeName grouped = ParseGroupedType(ref reader);
                return ParseTypePrefixRecursive(ref reader, grouped);
            }
            else if(reader.CurrentType == TokenType.Func)
            {
                return ParseFunctionPointerType(ref reader);
            }
            else if(reader.CurrentType == TokenType.Mut && reader.Peek() is Token t && t.Type == TokenType.Func)
            {
                return ParseFunctionPointerType(ref reader);
            }
            else
            {
                ParserError error = new ExpectedTypeNameError(reader.CurrentLocation());
                throw new ParserExeption(error);
            }
        }

        private static TypeName ParseFunctionPointerType(ref TokenReader reader)
        {
            Token funcToken = reader.Consume(TokenType.Func);
            Option<List<Token>> lifetimes = new Option<List<Token>>();
            if(reader.CurrentType == TokenType.LessThan)
            {
                reader.Consume(TokenType.LessThan);
                List<Token> ls = new List<Token>();
                while(true)
                {
                    ls.Add(reader.Consume(TokenType.Lifetime));

                    if (reader.Match(TokenType.GreaterThan))
                        break;

                    reader.Consume(TokenType.Comma);
                }

                lifetimes = ls;
            }

            Token openParen = reader.Consume(TokenType.OpenParen);
            List<TypeName> parameters = new List<TypeName>();
            while(!reader.Match(TokenType.CloseParen))
            {
                parameters.Add(ParseTypeName(ref reader));
                if (reader.CurrentType != TokenType.CloseParen)
                    reader.Consume(TokenType.Comma);
            }

            Token closeParen = reader.Previous();
            Token arrow = reader.Consume(TokenType.RightThinArrow);
            TypeName returnType = ParseTypeName(ref reader);

            return new FuncPtr(funcToken, lifetimes, openParen, parameters, closeParen, arrow, returnType);
        }

        private static TypeName ParseGroupedType(ref TokenReader reader)
        {
            Token openParen = reader.Consume(TokenType.OpenParen);
            TypeName type = ParseTypeName(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen);
            return new GroupedType(openParen, type, closeParen);
        }
    }
}
