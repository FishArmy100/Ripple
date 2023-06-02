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
                return ParseAssignment(reader);
            }
            catch(ParserExeption e)
            {
                return e.Error;
            }
        }

        private static Result<Expression, ParserError> ParseAssignment(TokenReader reader)
        {
            return ParseCasting(reader).Match(ok =>
            {
                if (ok is AST.Index || ok is Identifier || ok is Unary u && u.Op.Type == TokenType.Star) // is an index, identifier, or dereference
                {
                    if (reader.Match(TokenType.Equal))
                    {
                        Token equles = reader.Previous();
                        var value = ParseAssignment(reader);
                        if (value.IsError())
                            return value.Error;

                        return new Binary(ok, equles, value.Value);
                    }
                }

                return ok;
            },
            error => new Result<Expression, ParserError>(error));
        }

        private static Result<Expression, ParserError> ParseCasting(TokenReader reader)
        {
            return ParseLogicalOr(reader).Match(
                expression =>
                {
                    while (reader.Match(TokenType.As))
                    {
                        Token asToken = reader.Previous();
                        var typeName = TypeNameParser.ParseTypeName(ref reader);
                        if (typeName.IsError())
                            return typeName.Error;

                        expression = new Cast(expression, asToken, typeName.Value);
                    }

                    return expression;
                },
                error =>
                {
                    return new Result<Expression, ParserError>(error);
                });
        }

        private static Result<Expression, ParserError> ParseLogicalOr(TokenReader reader) => GetBinaryExpression(reader, ParseLogicalAnd, TokenType.PipePipe);
        private static Result<Expression, ParserError> ParseLogicalAnd(TokenReader reader)
        {
            return ParseEquality(reader).Match(
                expression =>
                {
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
                        var right = ParseEquality(reader);

                        if (right.IsError())
                            return right.Error;

                        expression = new Binary(expression, op, right.Value);
                    }

                    return expression;
                },
                error =>
                {
                    return new Result<Expression, ParserError>(error);
                });
            
        }

        private static Result<Expression, ParserError> ParseEquality(TokenReader reader) => GetBinaryExpression(reader, ParseComparison, TokenType.EqualEqual, TokenType.BangEqual);
        private static Result<Expression, ParserError> ParseComparison(TokenReader reader) => GetBinaryExpression(reader, ParseTerm, TokenType.GreaterThan, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.LessThanEqual);
        private static Result<Expression, ParserError> ParseTerm(TokenReader reader) => GetBinaryExpression(reader, ParseFactor, TokenType.Plus, TokenType.Minus);
        private static Result<Expression, ParserError> ParseFactor(TokenReader reader) => GetBinaryExpression(reader, ParseUnaryExpr, TokenType.Star, TokenType.Slash, TokenType.Mod);

        private static Result<Expression, ParserError> GetBinaryExpression(TokenReader reader, Func<TokenReader, Result<Expression, ParserError>> previouseExpr, params TokenType[] operatorTypes)
        {
            return previouseExpr(reader).Match(expression =>
            {
                while (reader.Match(operatorTypes))
                {
                    Token op = reader.Peek(-1).Value;
                    var right = previouseExpr(reader);
                    if (right.IsError())
                        return right.Error;

                    expression = new Binary(expression, op, right.Value);
                }

                return expression;
            },
            error =>
            {
                return new Result<Expression, ParserError>(error);
            });
            
        }

        private static Result<Expression, ParserError> ParseUnaryExpr(TokenReader reader)
        {
            if (reader.CheckSequence(TokenType.Ampersand, TokenType.Mut))
            {
                Token anpersand = reader.Advance();
                Token mut = reader.Advance();

                if (anpersand.HasSpaceAfter)
                    return new CannotHaveSpaceError(reader.CurrentLocation(), anpersand.Type);

                ParseUnaryExpr(reader).Match(
                    expression => 
                    {
                        SourceLocation location = anpersand.Location + mut.Location;
                        Token refMut = new Token(new Option<object>(), location, TokenType.RefMut, mut.HasSpaceAfter);
                        return new Unary(refMut, expression);
                    }, 
                    error =>
                    {
                        return new Result<Expression, ParserError>(error);
                    });
            }

            if (reader.Match(TokenType.Minus, TokenType.Bang, TokenType.Star, TokenType.Ampersand))
            {
                Token tok = reader.Previous();
                return ParseUnaryExpr(reader).Match(
                    expression => new Unary(tok, expression), 
                    error => new Result<Expression, ParserError>(error));
            }

            return ParseSecondary(ref reader);
        }

        private static Result<Expression, ParserError> ParseSecondary(ref TokenReader reader)
        {
            var expression = ParsePrimary(reader);
            if (expression.IsError())
                return expression.Error;

            return ParseSecondary(ref reader, expression.Value);
        }

        private static Result<Expression, ParserError> ParseSecondary(ref TokenReader reader, Expression previous)
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

        private static Result<Expression, ParserError> ParseCall(ref TokenReader reader, Expression callee)
        {
            Token openParen = reader.Previous();
            List<Expression> arguments = new List<Expression>();
            while (reader.Current().Type != TokenType.CloseParen)
            {
                var argument = ParseExpression(ref reader);
                if (argument.IsError())
                    return argument.Error;

                arguments.Add(argument.Value);

                if (reader.Current().Type != TokenType.CloseParen)
                    reader.Consume(TokenType.Comma);
            }
            var closeParen = reader.Consume(TokenType.CloseParen);
            if (closeParen.IsError())
                return closeParen.Error;

            Expression call = new Call(callee, openParen, arguments, closeParen.Value);
            return ParseSecondary(ref reader, call);
        }

        private static Result<Expression, ParserError> ParseIndex(ref TokenReader reader, Expression indexee)
        {
            Token openBracket = reader.Previous();
            var expression = ParseExpression(ref reader);
            if (expression.IsError())
                return expression.Error;

            var closeBracket = reader.Consume(TokenType.CloseBracket);

            if (closeBracket.IsError())
                return closeBracket.Error;

            Expression index = new AST.Index(indexee, openBracket, expression.Value, closeBracket.Value);
            return ParseSecondary(ref reader, index);
        }

        private static Result<Expression, ParserError> ParseMemberAccess(ref TokenReader reader, Expression accessed)
        {
            Token dot = reader.Previous();
            var memberName = reader.Consume(TokenType.Identifier);
            if (memberName.IsError())
                return memberName.Error;

            Expression expression = new MemberAccess(accessed, dot, memberName.Value);

            return ParseSecondary(ref reader, expression);
        }


        private static Result<Expression, ParserError> ParsePrimary(TokenReader reader)
        {
            if (reader.Match(TokenType.IntagerLiteral, TokenType.FloatLiteral, TokenType.True, TokenType.False, TokenType.StringLiteral, TokenType.CStringLiteral, TokenType.CharactorLiteral, TokenType.Nullptr))
                return new Literal(reader.Previous());

            if (reader.Match(TokenType.Identifier))
                return new Identifier(reader.Previous());

            var grouping = ParseGrouping(reader);
            if(grouping.HasValue())
            {
                return grouping.Value.Match(
                    ok => ok, 
                    error => new Result<Expression, ParserError>(error));
            }

            var initalizerList = ParseInitializerList(reader);
            if(initalizerList.HasValue())
            {
                return initalizerList.Value.Match(
                    ok => ok, 
                    error => new Result<Expression, ParserError>(error));
            }


            var sizeOf = TryParseSizeof(ref reader);
            if(sizeOf.HasValue())
            {
                return sizeOf.Value.Match(
                    ok => ok, 
                    error => new Result<Expression, ParserError>(error));
            }

            return new ExpectedExpressionError(reader.CurrentLocation());
        }

        private static Option<Result<Expression, ParserError>> TryParseSizeof(ref TokenReader reader)
        {
            if (reader.Match(TokenType.Sizeof))
            {
                Token sizeOfToken = reader.Previous();
                var lessThan = reader.Consume(TokenType.LessThan);

                if (lessThan.IsError())
                    return new Result<Expression, ParserError>(lessThan.Error);

                var typeName = TypeNameParser.ParseTypeName(ref reader);
                if (typeName.IsError())
                    return new Result<Expression, ParserError>(typeName.Error);

                var greaterThan = reader.Consume(TokenType.GreaterThan);
                if (greaterThan.IsError())
                    return new Result<Expression, ParserError>(greaterThan.Error);

                var openParen = reader.Consume(TokenType.OpenParen);
                if (openParen.IsError())
                    return new Result<Expression, ParserError>(openParen.Error);

                var closeParen = reader.Consume(TokenType.CloseParen);
                if (closeParen.IsError())
                    return new Result<Expression, ParserError>(closeParen.Error);

                SizeOf sizeOf = new SizeOf(sizeOfToken, lessThan.Value, typeName.Value, greaterThan.Value, openParen.Value, closeParen.Value);
                return new Result<Expression, ParserError>(sizeOf);
            }

            return new Option<Result<Expression, ParserError>>();
        }

        private static Option<Result<Expression, ParserError>> ParseInitializerList(TokenReader reader)
        {
            if (!reader.Match(TokenType.OpenBrace))
                return new Option<Result<Expression, ParserError>>();

            Token openBrace = reader.Previous();
            List<Expression> expressions = new List<Expression>();
            while (reader.Current().Type != TokenType.CloseBrace)
            {
                var expression = ParseExpression(ref reader);
                if (expression.IsError())
                    return new Option<Result<Expression, ParserError>>(expression.Error);

                expressions.Add(expression.Value);

                if (reader.Current().Type != TokenType.CloseBrace)
                    reader.Consume(TokenType.Comma);
            }

            var closeBrace = reader.Consume(TokenType.CloseBrace);
            if (closeBrace.IsError())
                return new Option<Result<Expression, ParserError>>(closeBrace.Error);

            InitializerList initializerList = new InitializerList(openBrace, expressions, closeBrace.Value);
            return new Option<Result<Expression, ParserError>>(initializerList);
        }

        private static Option<Result<Expression, ParserError>> ParseGrouping(TokenReader reader)
        {
            if (reader.Match(TokenType.OpenParen))
            {
                Token openParen = reader.Previous();
                var groupedExpr = ParseExpression(ref reader);
                if (groupedExpr.IsError())
                    return new Option<Result<Expression, ParserError>>(groupedExpr.Error);

                var closeParen = reader.Consume(TokenType.CloseParen);
                if (closeParen.IsError())
                    return new Option<Result<Expression, ParserError>>(closeParen.Error);

                Grouping group = new Grouping(openParen, groupedExpr.Value, closeParen.Value);
            }

            return new Option<Result<Expression, ParserError>>();
        }
    }
}
