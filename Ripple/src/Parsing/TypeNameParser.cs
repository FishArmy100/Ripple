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
            int initialIndex = typeReader.Index;

            var result = ParseTypeName(ref typeReader);
            if (result.IsError())
                return false;

            int newIndex = typeReader.Index;
            length = newIndex - initialIndex;
            return true;
        }

        public static Result<TypeName, ParserError> ParseTypeName(ref TokenReader reader)
        {
            if (reader.Current().Type.IsIdentifier())
                return ParseSimpleTypeName(ref reader);
            else
                return ParseComplexType(ref reader);
        }

        private static Result<TypeName, ParserError> ParseSimpleTypeName(ref TokenReader reader)
        {
            if(reader.CheckSequence(TokenType.Identifier, TokenType.LessThan, TokenType.Lifetime))
            {
                Token name = reader.Advance();
                Token lessThan = reader.Advance();
                List<Token> lifetimes = new List<Token>();
                lifetimes.Add(reader.Advance());

                while(!reader.CurrentIs(TokenType.GreaterThan))
                {
                    var comma = reader.Consume(TokenType.Comma);
                    if (comma.IsError())
                        return comma.Error;

                    var lifetime = reader.Consume(TokenType.Lifetime);
                    if (lifetime.IsError())
                        return lifetime.Error;

                    lifetimes.Add(lifetime.Value);
                }

                var greaterThan = reader.Advance();
                return new GenericType(name, lessThan, lifetimes, greaterThan);
            }

            var identifier = reader.Consume(TokenType.Identifier);
            if (identifier.IsError())
                return identifier.Error;

            TypeName type = new BasicType(identifier.Value);
            return ParseTypePrefixRecursive(ref reader, type);
        }

        private static Result<TypeName, ParserError> ParseTypePrefixRecursive(ref TokenReader reader, TypeName previous)
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
                    return new ExpectedTokenError(reader.Previous().Location, TokenType.Ampersand, TokenType.Star);

                Token openBracket = reader.Previous();
                var sizeToken = reader.Consume(TokenType.IntagerLiteral);
                if (sizeToken.IsError())
                    return sizeToken.Error;

                var closeBracket = reader.Consume(TokenType.CloseBracket);
                if (closeBracket.IsError())
                    return closeBracket.Error;

                TypeName arrType = new ArrayType(previous, openBracket, sizeToken.Value, closeBracket.Value);
                return ParseTypePrefixRecursive(ref reader, arrType);
            }
            else
            {
                return previous;
            }
        }

        private static Result<TypeName, ParserError> ParseComplexType(ref TokenReader reader)
        {
            if(reader.Current().Type == TokenType.OpenParen)
            {
                var grouped = ParseGroupedType(ref reader);
                if (grouped.IsError())
                    return grouped.Error;

                return ParseTypePrefixRecursive(ref reader, grouped.Value);
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
                return new ExpectedTypeNameError(reader.CurrentLocation());
            }
        }

        private static Result<TypeName, ParserError> ParseFunctionPointerType(ref TokenReader reader)
        {
            var funcToken = reader.Consume(TokenType.Func);
            if (funcToken.IsError())
                return funcToken.Error;

            Option<List<Token>> lifetimes = new Option<List<Token>>();
            if(reader.CurrentType == TokenType.LessThan)
            {
                reader.Consume(TokenType.LessThan);
                lifetimes = new List<Token>();
                while(true)
                {
                    var lifetime = reader.Consume(TokenType.Lifetime);
                    if (lifetime.IsError())
                        return lifetime.Error;

                    lifetimes.Value.Add(lifetime.Value);

                    if (reader.Match(TokenType.GreaterThan))
                        break;

                    reader.Consume(TokenType.Comma);
                }
            }

            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return openParen.Error;

            List<TypeName> parameters = new List<TypeName>();
            while(!reader.Match(TokenType.CloseParen))
            {
                var typeParameter = ParseTypeName(ref reader);
                if (typeParameter.IsError())
                    return typeParameter.Error;

                parameters.Add(typeParameter.Value);
                if (reader.CurrentType != TokenType.CloseParen)
                    reader.Consume(TokenType.Comma);
            }

            Token closeParen = reader.Previous();
            var arrow = reader.Consume(TokenType.RightThinArrow);
            if (arrow.IsError())
                return arrow.Error;

            var returnType = ParseTypeName(ref reader);
            if (returnType.IsError())
                return returnType.Error;

            return new FuncPtr(funcToken.Value, lifetimes, openParen.Value, parameters, closeParen, arrow.Value, returnType.Value);
        }

        private static Result<TypeName, ParserError> ParseGroupedType(ref TokenReader reader)
        {
            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return openParen.Error;

            var type = ParseTypeName(ref reader);
            if (type.IsError())
                return type.Error;

            var closeParen = reader.Consume(TokenType.CloseParen);
            if (closeParen.IsError())
                return closeParen.Error;

            return new GroupedType(openParen.Value, type.Value, closeParen.Value);
        }
    }
}
