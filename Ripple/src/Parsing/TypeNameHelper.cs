using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.Parsing
{
    static class TypeNameHelper
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
            if(reader.Current().Type == TokenType.Mut)
            {
                if (reader.Peek(1) is Token t && t.Type.IsIdentifier())
                    return ParseSimpleTypeName(ref reader);
                else
                    return ParseComplexType(ref reader);
            }

            if (reader.Current().Type.IsIdentifier())
                return ParseSimpleTypeName(ref reader);
            else
                return ParseComplexType(ref reader);
        }

        private static TypeName ParseSimpleTypeName(ref TokenReader reader)
        {
            Token? mut = null;
            if (reader.Match(TokenType.Mut))
                mut = reader.Previous();

            Token identifier = reader.Consume(TokenType.Identifier, "Expected an identifier.");
            TypeName type = new BasicType(mut, identifier);
            return ParseTypePrefixRecursive(ref reader, type);
        }

        private static TypeName ParseTypePrefixRecursive(ref TokenReader reader, TypeName previous)
        {
            Token? mut = null;
            if (reader.Match(TokenType.Mut))
                mut = reader.Previous();

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
                Token openBracket = reader.Previous();
                Token size = reader.Consume(TokenType.IntagerLiteral, "Expected a size.");
                Token closeBracket = reader.Consume(TokenType.CloseBracket, "Expected a ']'");
                TypeName arrType = new ArrayType(previous, mut, openBracket, size, closeBracket);
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
                throw new ParserExeption(reader.Current(), "Expected a type name.");
            }
        }

        private static TypeName ParseFunctionPointerType(ref TokenReader reader)
        {
            Token? mutToken = reader.Match(TokenType.Mut) ? reader.Previous() : null;
            Token funcToken = reader.Consume(TokenType.Func, "Expected 'func'.");
            Option<List<Token>> lifetimes = new Option<List<Token>>();
            if(reader.CurrentType == TokenType.LessThan)
            {
                reader.Consume(TokenType.LessThan, "Expected a '<'.");
                List<Token> ls = new List<Token>();
                while(true)
                {
                    ls.Add(reader.Consume(TokenType.Lifetime, "Expected a lifetime parameter."));

                    if (reader.Match(TokenType.GreaterThan))
                        break;

                    reader.Consume(TokenType.Comma, "Expected a ','.");
                }

                lifetimes = ls;
            }

            Token openParen = reader.Consume(TokenType.OpenParen, "Expected a '('.");
            List<TypeName> parameters = new List<TypeName>();
            while(!reader.Match(TokenType.CloseParen))
            {
                parameters.Add(ParseTypeName(ref reader));
                if (reader.CurrentType != TokenType.CloseParen)
                    reader.Consume(TokenType.Comma, "Expected a ','.");
            }

            Token closeParen = reader.Previous();
            Token arrow = reader.Consume(TokenType.RightThinArrow, "Expected a '->'.");
            TypeName returnType = ParseTypeName(ref reader);

            return new FuncPtr(mutToken, funcToken, lifetimes, openParen, parameters, closeParen, arrow, returnType);
        }

        private static TypeName ParseGroupedType(ref TokenReader reader)
        {
            Token openParen = reader.Consume(TokenType.OpenParen, "Expected a '('.");
            TypeName type = ParseTypeName(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen, "Expected a ')'.");
            return new GroupedType(openParen, type, closeParen);
        }
    }
}
