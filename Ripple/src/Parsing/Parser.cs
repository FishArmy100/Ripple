using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;
using Ripple.Utils.Extensions;

namespace Ripple.Parsing
{
    static class Parser
    {
        public static (Expression, List<ParserError>) Parse(List<Token> tokens)
        {
            TokenReader reader = new TokenReader(tokens);
            List<ParserError> errors = new List<ParserError>();
            try
            {
                Expression expr = ParseExpression(ref reader);
                return (expr, errors);
            }
            catch(ParserExeption e)
            {
                errors.Add(new ParserError(e.Message, e.Tok));
                return (null, errors);
            }
        }

        private static Expression ParseExpression(ref TokenReader reader)
        {
            return ParseAssignment(ref reader);
        }

        private static Expression ParseAssignment(ref TokenReader reader)
        {
            TokenType currentType = reader.Current().Type;
            if(currentType.IsIdentifier() && reader.Peek() is Token t && t.Type == TokenType.Equal)
            {
                Token name = reader.Advance();
                Token equles = reader.Advance();
                Expression expr = ParseAssignment(ref reader);
                return new Binary(new Identifier(name), equles, expr);
            }
            else
            {
                return ParseLogicalOr(reader);
            }
        }

        private static Expression ParseLogicalOr(TokenReader reader) => GetBinaryExpression(reader, ParseLogicalAnd, TokenType.PipePipe);
        private static Expression ParseLogicalAnd(TokenReader reader) => GetBinaryExpression(reader, ParseEquality, TokenType.AmpersandAmpersand);
        private static Expression ParseEquality(TokenReader reader) => GetBinaryExpression(reader, ParseComparison, TokenType.EqualEqual, TokenType.BangEqual);
        private static Expression ParseComparison(TokenReader reader) => GetBinaryExpression(reader, ParseTerm, TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.LessThanEqual);
        private static Expression ParseTerm(TokenReader reader) => GetBinaryExpression(reader, ParseFactor, TokenType.Plus, TokenType.Minus);
        private static Expression ParseFactor(TokenReader reader) => GetBinaryExpression(reader, ParseUnaryExpr, TokenType.Star, TokenType.Slash);

        private static Expression GetBinaryExpression(TokenReader reader, Func<TokenReader, Expression> previouseExpr, params TokenType[] operatorTypes)
        {
            Expression expr = previouseExpr(reader);
            while (reader.Match(operatorTypes))
            {
                Token op = reader.Peek(-1).Value;
                Expression right = previouseExpr(reader);
                return new Binary(expr, op, right);
            }

            return expr;
        }

        private static Expression ParseUnaryExpr(TokenReader reader)
        {
            if(reader.Match(TokenType.Minus, TokenType.Bang))
            {
                Token tok = reader.Previous();
                Expression expr = ParseUnaryExpr(reader);
                return new Unary(tok, expr);
            }

            return ParseCall(reader);
        }

        private static Expression ParseCall(TokenReader reader)
        {
            Expression expr = ParsePrimary(reader);
            if(reader.Current().Type == TokenType.OpenParen)
            {
                Token openParen = reader.Advance();
                List<Expression> args = new List<Expression>();

                while(!reader.Match(TokenType.CloseParen))
                {
                    args.Add(ParseExpression(ref reader));
                    if (reader.Current().Type != TokenType.CloseParen)
                        reader.Consume(TokenType.Comma, "Expected ',' to seperate arguments.");
                }

                Token closeParen = reader.Peek(-1).Value;

                return new Call(expr, openParen, args, closeParen);
            }

            return expr;
        }

        private static Expression ParsePrimary(TokenReader reader)
        {
            if (reader.Match(TokenType.IntagerLiteral, TokenType.FloatLiteral, TokenType.True, TokenType.False))
                return new Literal(reader.Previous());
            else if (reader.Match(TokenType.Identifier))
                return new Identifier(reader.Previous());
            else if (ParseGrouping(reader, out Expression expr))
                return expr;
            else 
                throw new ParserExeption(reader.Current(), "Expected an expression.");
        }

        private static bool ParseGrouping(TokenReader reader, out Expression expr)
        {
            if(reader.Match(TokenType.OpenParen))
            {
                Token openParen = reader.Previous();
                Expression groupedExpr = ParseExpression(ref reader);
                Token closeParen = reader.Consume(TokenType.CloseParen, "Expected ')' after grouped expression");
                expr = new Grouping(openParen, groupedExpr, closeParen);
                return true;
            }

            expr = null;
            return false;
        }

        // Extensions
        private static Token Consume(this TokenReader reader, TokenType type, string message)
        {
            if(reader.Match(type))
            {
                return reader.Peek(-1).Value;
            }

            throw new ParserExeption(reader.Current(), message);
        }
    }
}
