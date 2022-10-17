using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;
using Ripple.Utils.Extensions;

namespace Ripple.Parsing
{
    static class TypeNameHelper
    {
        public static bool IsTypeName(ref TokenReader reader, out int length)
        {
            length = 0;
            TokenReader typeReader = new TokenReader(reader.GetTokens(), reader.Index);
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
                TypeName refType = new ReferenceType(previous, mut, reader.Previous());
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
            Token? mutToken = null;
            if (reader.Match(TokenType.Mut))
                mutToken = reader.Previous();

            Token openParen = reader.Consume(TokenType.OpenParen, "Expected '('.");
            List<TypeName> typeNames = new List<TypeName>();
            while(!reader.Match(TokenType.CloseParen))
            {
                typeNames.Add(ParseTypeName(ref reader));
                if(!reader.Current().IsType(TokenType.CloseParen))
                {
                    reader.Consume(TokenType.Comma, "Expected ','.");
                }
            }
            Token closeParen = reader.Previous();

            if(reader.Match(TokenType.RightThinArrow)) // is a function pointer
            {
                Token arrow = reader.Previous();
                TypeName returnType = ParseTypeName(ref reader);
                return new FuncPtr(mutToken, openParen, typeNames, closeParen, arrow, returnType);
            }
            else if(typeNames.Count == 1) // is a grouped type
            {
                if (mutToken != null)
                    throw new ParserExeption(mutToken.Value, "Grouped type cannot have 'mut' qualifier");

                return new GroupedType(openParen, typeNames[0], closeParen);
            }
            else if(typeNames.Count == 0)
            {
                throw new ParserExeption(closeParen, "Expected a type name");
            }
            else
            {
                throw new ParserExeption(closeParen, "Expected a function pointer");
            }
        }
    }
}
