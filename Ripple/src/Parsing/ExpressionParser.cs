using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;
using Ripple.AST;
using Ripple.Parsing.Errors;
using Ripple.Lexing;
using Ripple.Core;

namespace Ripple.Parsing
{
    static class ExpressionParser
    {
        public static Result<Expression, ParserError> ParseExpression(ref TokenReader reader)
        {
            try
            {
                return ParseAssignment(ref reader);
            }
            catch(ParserExeption e)
            {
                return e.Error;
            }
        }

        public static Expression ParseExpressionOrThrow(ref TokenReader reader)
        {
            return ParseExpression(ref reader).Match(
                ok => ok,
                fail => throw new ParserExeption(fail));
        }

        private static Expression ParseAssignment(ref TokenReader reader)
        {
            Expression obj = ParseCasting(ref reader);
            if (obj is AST.Index || obj is Identifier || obj is Unary u && u.Op.Type == TokenType.Star) // is an index, identifier, or dereference
            {
                if (reader.Match(TokenType.Equal))
                {
                    Token equles = reader.Previous();
                    Expression value = ParseAssignment(ref reader);
                    return new Binary(obj, equles, value);
                }
            }

            return obj;
        }

        private static Expression ParseCasting(ref TokenReader reader)
        {
            Expression expression = ParseLogicalOr(reader);
            while (reader.Match(TokenType.As))
            {
                Token asToken = reader.Previous();
                TypeName typeName = TypeNameParser.ParseTypeName(ref reader);
                expression = new Cast(expression, asToken, typeName);
            }

            return expression;
        }

        private static Expression ParseLogicalOr(TokenReader reader) => GetBinaryExpression(reader, ParseLogicalAnd, TokenType.PipePipe);
        private static Expression ParseLogicalAnd(TokenReader reader)
        {
            Expression expr = ParseEquality(reader);
            while (reader.CheckSequence(TokenType.Ampersand, TokenType.Ampersand))
            {
                Token anpersand1 = reader.Advance();
                Token anpersand2 = reader.Advance();

                if (anpersand1.HasSpaceAfter)
                {
                    ParserError error = new CannotHaveSpaceError(reader.CurrentLocation(), anpersand1.Type);
                    throw new ParserExeption(error);
                }

                SourceLocation location = anpersand1.Location + anpersand2.Location;

                Token op = new Token(new Option<object>(), location, TokenType.AmpersandAmpersand, anpersand2.HasSpaceAfter);
                Expression right = ParseEquality(reader);
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private static Expression ParseEquality(TokenReader reader) => GetBinaryExpression(reader, ParseComparison, TokenType.EqualEqual, TokenType.BangEqual);
        private static Expression ParseComparison(TokenReader reader) => GetBinaryExpression(reader, ParseTerm, TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.LessThanEqual);
        private static Expression ParseTerm(TokenReader reader) => GetBinaryExpression(reader, ParseFactor, TokenType.Plus, TokenType.Minus);
        private static Expression ParseFactor(TokenReader reader) => GetBinaryExpression(reader, ParseUnaryExpr, TokenType.Star, TokenType.Slash, TokenType.Mod);

        private static Expression GetBinaryExpression(TokenReader reader, Func<TokenReader, Expression> previouseExpr, params TokenType[] operatorTypes)
        {
            Expression expr = previouseExpr(reader);
            while (reader.Match(operatorTypes))
            {
                Token op = reader.Peek(-1).Value;
                Expression right = previouseExpr(reader);
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private static Expression ParseUnaryExpr(TokenReader reader)
        {
            if (reader.CheckSequence(TokenType.Ampersand, TokenType.Mut))
            {
                Token anpersand = reader.Advance();
                Token mut = reader.Advance();

                if (anpersand.HasSpaceAfter)
                {
                    ParserError error = new CannotHaveSpaceError(reader.CurrentLocation(), anpersand.Type);
                    throw new ParserExeption(error);
                }

                Expression expr = ParseUnaryExpr(reader);

                SourceLocation location = anpersand.Location + mut.Location;

                Token refMut = new Token(new Option<object>(), location, TokenType.RefMut, mut.HasSpaceAfter);
                return new Unary(refMut, expr);
            }

            if (reader.Match(TokenType.Minus, TokenType.Bang, TokenType.Star, TokenType.Ampersand))
            {
                Token tok = reader.Previous();
                Expression expr = ParseUnaryExpr(reader);
                return new Unary(tok, expr);
            }

            return ParseSecondary(ref reader);
        }

        private static Expression ParseSecondary(ref TokenReader reader)
        {
            Expression expression = ParsePrimary(reader);
            return ParseSecondary(ref reader, expression);
        }

        private static Expression ParseSecondary(ref TokenReader reader, Expression previous)
        {
            if(reader.Match(TokenType.OpenParen))
            {
                return ParseCall(ref reader, previous);
            }
            else if(reader.Match(TokenType.OpenBracket))
            {
                return ParseIndex(ref reader, previous);
            }
            else if(reader.Match(TokenType.Dot))
            {
                return ParseMemberAccess(ref reader, previous);
            }

            return previous;
        }

        private static Expression ParseCall(ref TokenReader reader, Expression callee)
        {
            Token openParen = reader.Previous();
            List<Expression> arguments = new List<Expression>();
            while (reader.Current().Type != TokenType.CloseParen)
            {
                arguments.Add(ParseExpressionOrThrow(ref reader));

                if (reader.Current().Type != TokenType.CloseParen)
                    reader.Consume(TokenType.Comma);
            }
            Token closeParen = reader.Consume(TokenType.CloseParen);
            Expression call = new Call(callee, openParen, arguments, closeParen);
            return ParseSecondary(ref reader, call);
        }

        private static Expression ParseIndex(ref TokenReader reader, Expression indexee)
        {
            Token openBracket = reader.Previous();
            Expression expression = ParseExpressionOrThrow(ref reader);
            Token closeBracket = reader.Consume(TokenType.CloseBracket);
            Expression index = new AST.Index(indexee, openBracket, expression, closeBracket);
            return ParseSecondary(ref reader, index);
        }

        private static Expression ParseMemberAccess(ref TokenReader reader, Expression accessed)
        {
            Token dot = reader.Previous();
            Token memberName = reader.Consume(TokenType.Identifier);
            Expression expression = new MemberAccess(accessed, dot, memberName);

            return ParseSecondary(ref reader, expression);
        }


        private static Expression ParsePrimary(TokenReader reader)
        {
            if (reader.Match(TokenType.IntagerLiteral, TokenType.FloatLiteral, TokenType.True, TokenType.False, TokenType.StringLiteral, TokenType.CStringLiteral, TokenType.CharactorLiteral, TokenType.Nullptr))
                return new Literal(reader.Previous());
            else if (reader.Match(TokenType.Identifier))
                return new Identifier(reader.Previous());
            else if (ParseGrouping(reader, out Expression expr))
                return expr;
            else if (ParseInitializerList(reader, out expr))
                return expr;
            else if (TryParseSizeof(ref reader, out expr))
                return expr;
            else
            {
                ParserError error = new ExpectedExpressionError(reader.CurrentLocation());
                throw new ParserExeption(error);
            }
        }

        private static bool TryParseSizeof(ref TokenReader reader, out Expression sizeOf)
        {
            sizeOf = null;
            if (reader.Match(TokenType.Sizeof))
            {
                Token sizeOfToken = reader.Previous();
                Token lessThan = reader.Consume(TokenType.LessThan);
                TypeName typeName = TypeNameParser.ParseTypeName(ref reader);
                Token greaterThan = reader.Consume(TokenType.GreaterThan);
                Token openParen = reader.Consume(TokenType.OpenParen);
                Token closeParen = reader.Consume(TokenType.CloseParen);

                sizeOf = new SizeOf(sizeOfToken, lessThan, typeName, greaterThan, openParen, closeParen);
                return true;
            }

            return false;
        }

        private static bool ParseInitializerList(TokenReader reader, out Expression initalizerList)
        {
            initalizerList = null;
            if (!reader.Match(TokenType.OpenBrace))
                return false;

            Token openBrace = reader.Previous();
            List<Expression> expressions = new List<Expression>();
            while (reader.Current().Type != TokenType.CloseBrace)
            {
                expressions.Add(ParseExpressionOrThrow(ref reader));

                if (reader.Current().Type != TokenType.CloseBrace)
                    reader.Consume(TokenType.Comma);
            }
            Token closeBrace = reader.Consume(TokenType.CloseBrace);

            initalizerList = new InitializerList(openBrace, expressions, closeBrace);
            return true;
        }

        private static bool ParseGrouping(TokenReader reader, out Expression expr)
        {
            if (reader.Match(TokenType.OpenParen))
            {
                Token openParen = reader.Previous();
                Expression groupedExpr = ParseExpressionOrThrow(ref reader);

                Token closeParen = reader.Consume(TokenType.CloseParen);
                expr = new Grouping(openParen, groupedExpr, closeParen);
                return true;
            }

            expr = null;
            return false;
        }
    }
}
