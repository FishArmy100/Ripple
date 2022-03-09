using System;
using System.Collections.Generic;
using System.Text;
using Ripple.Utils;

namespace Ripple
{
    static class Parser
    {
        public static OperationResult<AbstractSyntaxTree, ParserError> Parse(List<Token> tokens)
        {
            TokenReader reader = new TokenReader(tokens);
            List<Statement> statements = new List<Statement>();
            List<ParserError> errors = new List<ParserError>();

            while (!reader.IsAtEnd())
            {
                Statement decl = Declaration(reader, out ParserError error);
                if (decl != null)
                    statements.Add(decl);
                else
                    errors.Add(error);
            }

            AbstractSyntaxTree ast = new AbstractSyntaxTree(statements);
            return new OperationResult<AbstractSyntaxTree, ParserError>(ast, errors);
        }

        private static Statement Declaration(TokenReader reader, out ParserError error)
        {
            error = new ParserError();

            try
            {
                return Declaration(reader);
            }
            catch (ParserException e)
            {
                error = e.Error;
                reader.AdvanceCurrent();
                //Synchronize(); TODO: Implement
                return null;
            }
        }

        private static Statement Declaration(TokenReader reader)
        {
            if (IsVariableDeclaration(reader))
                return VariableDeclaration(reader);

            return CreateStatement(reader);
        }

        private static Statement CreateStatement(TokenReader reader)
        {
            if (reader.MatchCurrent(TokenType.While))
                return WhileLoop(reader);

            if (reader.MatchCurrent(TokenType.For))
                return ForLoop(reader);

            if (reader.MatchCurrent(TokenType.If))
                return IfStatement(reader);

            if (reader.MatchCurrent(TokenType.OpenBrace))
                return Block(reader);

            if (reader.MatchCurrent(TokenType.Continue))
                return ContinueStatement(reader);

            if (reader.MatchCurrent(TokenType.Break))
                return BreakStatement(reader);

            return ExpressionStatment(reader);
        }

        private static Statement WhileLoop(TokenReader reader)
        {
            Consume(reader, TokenType.OpenParen, "Expect '(' after 'for'.");

            Expression condition = null;
            if (!reader.CheckCurrent(TokenType.Semicolon))
                condition = CreateExpression(reader);
            Consume(reader, TokenType.CloseParen, "Expect ')' after while clause");

            Statement body = CreateStatement(reader);
            return new Statement.WhileLoop(condition, body);
        }

        private static Statement ForLoop(TokenReader reader)
        {
            Consume(reader, TokenType.OpenParen, "Expect '(' after 'for'.");

            Statement initializer = null;
            if (IsVariableDeclaration(reader))
                initializer = VariableDeclaration(reader);
            else
                initializer = ExpressionStatment(reader);

            Expression condition = null;
            if (!reader.CheckCurrent(TokenType.Semicolon))
                condition = CreateExpression(reader);
            Consume(reader, TokenType.Semicolon, "Expect ';' after loop condition");

            Expression increment = null;
            if (!reader.CheckCurrent(TokenType.CloseParen))
                increment = CreateExpression(reader);
            Consume(reader, TokenType.CloseParen, "Expect ')' after for clauses");

            Statement body = CreateStatement(reader);

            return new Statement.ForLoop(initializer, condition, increment, body);
        }

        private static Statement Block(TokenReader reader)
        {
            List<Statement> statements = new List<Statement>();

            while (!reader.CheckCurrent(TokenType.CloseBrace) && !reader.IsAtEnd())
                statements.Add(Declaration(reader));

            Consume(reader, TokenType.CloseBrace, "Expect '}' after block statement");
            return new Statement.Block(statements);
        }

        private static Statement IfStatement(TokenReader reader)
        {
            Consume(reader, TokenType.OpenParen, "Expect '(' after 'if'");
            Expression condition = CreateExpression(reader);
            Consume(reader, TokenType.CloseParen, "Expect ')' after if condition");

            Statement thenBranch = CreateStatement(reader);

            Statement elseBranch = null;
            if (reader.MatchCurrent(TokenType.Else))
                elseBranch = CreateStatement(reader);

            return new Statement.IfStmt(condition, thenBranch, elseBranch);
        }

        private static Statement ContinueStatement(TokenReader reader)
        {
            Token token = reader.Previous();
            Consume(reader, TokenType.Semicolon, "Expected ';' after 'continue'");
            return new Statement.ContinueStmt(token);
        }

        private static Statement BreakStatement(TokenReader reader)
        {
            Token token = reader.Previous();
            Consume(reader, TokenType.Semicolon, "Expected ';' after 'break'");
            return new Statement.BreakStmt(token);
        }

        private static Statement VariableDeclaration(TokenReader reader)
        {
            Token type = reader.AdvanceCurrent();
            Token name = reader.AdvanceCurrent();
            Expression initializer = null;

            if (reader.MatchCurrent(TokenType.Equal))
                initializer = CreateExpression(reader);

            Consume(reader, TokenType.Semicolon, "Expect ';' after variable declaration.");
            return new Statement.VarDeclaration(type, name, initializer);
        }

        private static Statement ExpressionStatment(TokenReader reader)
        {
            Expression expr = CreateExpression(reader);
            Consume(reader, TokenType.Semicolon, "Expect ';' after expression");
            return new Statement.ExpressionStmt(expr);
        }

        private static Expression CreateExpression(TokenReader reader)
        {
            return Assignment(reader);
        }

        private static Expression Assignment(TokenReader reader)
        {
            Expression expr = LogicOr(reader);
            if(reader.MatchCurrent(TokenType.Equal))
            {
                Token equal = reader.Previous();
                Expression value = Assignment(reader);

                if(expr is Expression.Variable variable)
                {
                    Token name = variable.Name;
                    return new Expression.Assignment(name, value);
                }

                throw new ParserException("Invalid assignment target.", equal, reader.Current);
            }

            return expr;
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
            if(reader.MatchCurrent(TokenType.IntLiteral, TokenType.FloatLiteral, TokenType.CharLiteral, TokenType.StringLiteral, TokenType.True, TokenType.False, TokenType.Null))
            {
                return new Expression.Literal(reader.Previous());
            }

            if(reader.MatchCurrent(TokenType.OpenParen))
            {
                Expression expr = CreateExpression(reader);
                Consume(reader, TokenType.CloseParen, "Expect ')' after expression.");
                return new Expression.Grouping(expr);
            }

            if(reader.MatchCurrent(TokenType.Identifier))
            {
                return new Expression.Variable(reader.Previous());
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

        private static bool IsVariableDeclaration(TokenReader reader)
        {
            Token type = reader.PeekCurrent();
            Token name = reader.PeekCurrent(1);
            return (type.Type == TokenType.Identifier || type.Type.IsBuiltInType()) && name.Type == TokenType.Identifier;
        }
    }
}
