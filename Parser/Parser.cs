using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    static class Parser
    {
        public static ParserResult Parse(List<Token> tokens)
        {
            TokenReader reader = new TokenReader(tokens);
            try
            {
                Expression expr = CreateExpression(reader);
                if(!reader.IsAtEnd())
                    return new ParserResult(expr, new List<ParserError>() { new ParserError("More than one expression present", Token.Invalid, 0) });
                return new ParserResult(expr, new List<ParserError>());
            }
            catch(ParserException e)
            {
                return new ParserResult(null, new List<ParserError>() { e.Error });
            }
        }

        private static Expression CreateExpression(TokenReader reader)
        {
            return LogicOr(reader);
        }

        private static Expression LogicOr(TokenReader reader) => GetBinaryExpression(reader, LogicAnd, TokenType.PipePipe); 
        private static Expression LogicAnd(TokenReader reader) => GetBinaryExpression(reader, Equality, TokenType.AmpersandAmpersand);
        private static Expression Equality(TokenReader reader) => GetBinaryExpression(reader, Comparison, TokenType.EqualEqual, TokenType.BangEqual);
        private static Expression Comparison(TokenReader reader) => GetBinaryExpression(reader, Term, TokenType.LessThen, TokenType.LessThenEqual, TokenType.GreaterThen, TokenType.GreaterThenEqual);
        private static Expression Term(TokenReader reader) => GetBinaryExpression(reader, Factor, TokenType.Plus, TokenType.Minus);
        private static Expression Factor(TokenReader reader) => GetBinaryExpression(reader, Unary, TokenType.Star, TokenType.Slash, TokenType.Percent);

        private static Expression GetBinaryExpression(TokenReader reader, Func<TokenReader, Expression> previouseExpr, params TokenType[] operatorTypes)
        {
            Expression expr = previouseExpr(reader);
            while(reader.MatchCurrent(operatorTypes))
            {
                Token op = reader.Previous();
                Expression right = previouseExpr(reader);
                return new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private static Expression Unary(TokenReader reader)
        {
            if(reader.MatchCurrent(TokenType.Minus, TokenType.Bang))
            {
                Token op = reader.Previous();
                Expression right = Unary(reader);
                return new Expression.Unary(right, op);
            }

            return Primary(reader);
        }

        private static Expression Primary(TokenReader reader)
        {
            if(reader.MatchCurrent(TokenType.Int, TokenType.Float, TokenType.True, TokenType.False, TokenType.Null))
            {
                return new Expression.Literal(reader.Previous());
            }

            if(reader.MatchCurrent(TokenType.OpenParen))
            {
                Expression expr = CreateExpression(reader);
                Consume(reader, TokenType.CloseParen, "Expect ')' after expression.");
                return new Expression.Grouping(expr);
            }

            Token token = reader.PeekCurrent();

            if (reader.PeekCurrent().Type == TokenType.Invalid)
                throw new ParserException("Invalid token at line: " + token.Line, token, reader.Current);

            throw new ParserException("Invalid primary token: \"" + token.Lexeme + "\" at line " + token.Line, token, reader.Current);
        }

        private static Token Consume(TokenReader reader, TokenType type, string errorMessage)
        {
            if (reader.CheckCurrent(type))
                return reader.AdvanceCurrent();

            throw new ParserException(errorMessage, reader.PeekCurrent(), reader.Current);
        }
    }
}
