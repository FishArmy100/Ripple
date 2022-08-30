using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.Parsing
{
    static class Parser
    {
        public static Result<List<Statement>, List<ParserError>> Parse(List<Token> tokens)
        {
            TokenReader reader = new TokenReader(tokens);
            List<ParserError> errors = new List<ParserError>();
            List<Statement> statements = new List<Statement>();

            while(!reader.IsAtEnd() && reader.Current().Type != TokenType.EOF)
            {
                try
                {
                    Statement statement = ParseStatement(ref reader, ref errors);
                    statements.Add(statement);
                }
                catch(ParserExeption e)
                {
                    errors.Add(new ParserError(e.Message, e.Tok));
                    reader.SyncronizeTo(TokenType.If, TokenType.For, TokenType.Return, TokenType.OpenBrace);
                }
            }

            if (errors.Count > 0)
                return new Result<List<Statement>, List<ParserError>>.Fail(errors);

            return new Result<List<Statement>, List<ParserError>>.Ok(statements);
        }

        private static Statement ParseStatement(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (TryParseIf(ref reader, ref errors, out Statement statement))
                return statement;
            else if (TryParseBlock(ref reader, ref errors, out statement))
                return statement;
            else if (TryParseFor(ref reader, ref errors, out statement))
                return statement;
            else if (TryParseReturn(ref reader, out statement))
                return statement;
            else if (TryParseVarDecl(ref reader, out statement))
                return statement;
            else
                return ParseExpressionStatement(ref reader);
        }

        private static bool TryParseIf(ref TokenReader reader, ref List<ParserError> errors, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.If))
                return false;

            Token ifToken = reader.Previous();
            Token openParen = reader.Consume(TokenType.OpenParen, "Expected '('.");
            Expression expr = ParseExpression(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen, "Expected ')'.");
            Statement body = ParseStatement(ref reader, ref errors);

            statement = new IfStmt(ifToken, openParen, expr, closeParen, body);
            return true;
        }

        private static bool TryParseFor(ref TokenReader reader, ref List<ParserError> errors, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.For))
                return false;

            Token forToken = reader.Previous();

            Token openParen = reader.Consume(TokenType.OpenParen, "Expected '('.");

            TryParseVarDecl(ref reader, out Statement init);

            Expression condition = null;
            if (reader.Current().Type != TokenType.SemiColon)
                condition = ParseExpression(ref reader);
            reader.Consume(TokenType.SemiColon, "Expected ';'.");

            Expression itr = null;
            if (reader.Current().Type != TokenType.CloseParen)
                itr = ParseExpression(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen, "Expected ')'.");

            Statement body = ParseStatement(ref reader, ref errors);

            statement = new ForStmt(forToken, openParen, init, condition, itr, closeParen, body);
            return true;
        }

        public static bool TryParseReturn(ref TokenReader reader, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.Return))
                return false;

            Token returnToken = reader.Previous();
            Expression expr = ParseExpression(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon, "Expected ';' after return statement");

            statement = new ReturnStmt(returnToken, expr, semiColon);
            return true;
        }

        public static bool TryParseVarDecl(ref TokenReader reader, out Statement statement)
        {
            statement = null;
            if (!reader.Current().Type.IsIdentifier() || !(reader.Peek() is Token t && t.Type.IsIdentifier()))
                return false;

            Token varType = reader.Advance();
            List<Token> varNames = new List<Token>();
            varNames.Add(reader.Advance());

            while(reader.Match(TokenType.Comma))
            {
                varNames.Add(reader.Consume(TokenType.Identifier, "Expected variable name."));
            }

            Token equel = reader.Consume(TokenType.Equal, "Expected '='.");

            Expression expr = ParseExpression(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon, "Expected ';' after variable declaration.");

            statement = new VarDecl(varType, varNames, equel, expr, semiColon);
            return true;
        }

        private static bool TryParseBlock(ref TokenReader reader, ref List<ParserError> errors, out Statement statement)
        {
            if(reader.Match(TokenType.OpenBrace))
            {
                Token openBrace = reader.Previous();

                List<Statement> statements = new List<Statement>();

                while (!reader.IsAtEnd() && !reader.Match(TokenType.CloseBrace))
                {
                    try { statements.Add(ParseStatement(ref reader, ref errors)); }
                    catch(ParserExeption e) 
                    { 
                        errors.Add(new ParserError(e.Message, e.Tok));
                        reader.SyncronizeTo(TokenType.For, TokenType.If, TokenType.CloseBrace, TokenType.OpenBrace);
                    }
                }

                Token closeBrace = reader.Previous();

                statement = new BlockStmt(openBrace, statements, closeBrace);
                return true;
            }

            statement = null;
            return false;
        }

        private static Statement ParseExpressionStatement(ref TokenReader reader)
        {
            Expression expr = ParseExpression(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon, "Expected ';' after expression.");
            return new ExprStmt(expr, semiColon);
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
        private static Expression ParseFactor(TokenReader reader) => GetBinaryExpression(reader, ParseUnaryExpr, TokenType.Star, TokenType.Slash, TokenType.Mod);

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
                    {
                        reader.Consume(TokenType.Comma, "Expected ',' to seperate arguments");
                        args.Add(ParseExpression(ref reader));
                    }
                }

                Token closeParen = reader.Previous();

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
            {
                throw new ParserExeption(reader.Current(), "Expected expression");
            }
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

        private static Token Consume(this TokenReader reader, TokenType tokenType, string errorMessage)
        {
            if (reader.Match(tokenType))
                return reader.Previous();

            throw new ParserExeption(reader.Current(), errorMessage);
        }

        private static void SyncronizeTo(this TokenReader reader, params TokenType[] types)
        {
            while(true)
            {
                if (reader.IsAtEnd() || types.Contains(reader.Current().Type))
                    break;

                reader.Advance();
            }
        }
    }
}
