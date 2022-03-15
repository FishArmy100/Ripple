using System;
using System.Collections.Generic;
using System.Text;
using Ripple.Utils;

namespace Ripple
{
    static class Parser
    {
        private static readonly TokenType[] MemberAttributes = { TokenType.Public, TokenType.Protected, TokenType.Private, TokenType.Virtual, TokenType.Override };

        public static OperationResult<AbstractSyntaxTree, ParserError> Parse(List<Token> tokens)
        {
            TokenReader reader = new TokenReader(tokens);
            List<Statement> statements = new List<Statement>();
            List<ParserError> errors = new List<ParserError>();

            while (!reader.IsAtEnd())
            {
                try
                {
                    Statement declaration = CreateDeclaration(reader);
                    statements.Add(declaration);
                }
                catch(ParserException e)
                {
                    errors.Add(e.Error);
                    reader.AdvanceCurrent();
                    Synchronize(reader);
                }
            }

            AbstractSyntaxTree ast = new AbstractSyntaxTree(statements);
            return new OperationResult<AbstractSyntaxTree, ParserError>(ast, errors);
        }

        // ------------------------------------------------------------------------------------
        // Declaration:
        // ------------------------------------------------------------------------------------
        private static Statement CreateDeclaration(TokenReader reader)
        {
            if (IsCurrentVariableDeclaration(reader))
                return VariableDeclaration(reader);

            if (IsCurrentFunctionDeclaration(reader))
                return FunctionDeclaration(reader);

            if (IsCurrentClassDeclaration(reader))
                return ClassDeclaration(reader);

            throw new ParserException("Expected declaration", reader.PeekCurrent(), reader.Current);
        }

        private static Statement VariableDeclaration(TokenReader reader)
        {
            Token type = reader.AdvanceCurrent();
            Token name = reader.AdvanceCurrent();
            Expression initializer = null;

            if (reader.MatchCurrent(TokenType.Equal))
                initializer = CreateExpression(reader);

            Consume(reader, TokenType.Semicolon, "Expect ';' after variable declaration.");
            return new VariableDecl(type, name, initializer);
        }

        private static Statement FunctionDeclaration(TokenReader reader)
        {
            Token returnType = reader.AdvanceCurrent();
            Token name = reader.AdvanceCurrent();

            List<FunctionParameter> parameters = ReadParameters(reader);

            Consume(reader, TokenType.OpenBrace, "Expect '{' after function definition.");
            BlockStmt body = (BlockStmt)Block(reader);
            return new FunctionDecl(returnType, name, parameters, body);
        }

        private static Statement ClassDeclaration(TokenReader reader)
        {
            reader.AdvanceCurrent(); // gets rid of class

            Token name = Consume(reader, TokenType.Identifier, "Expected class name.");
            Token? baseName = null;

            if (reader.MatchCurrent(TokenType.Colon))
                baseName = Consume(reader, TokenType.Identifier, "Expected identifier as base name.");

            Consume(reader, TokenType.OpenBrace, "Expected '{' after class name");

            List<MemberDeclarationStmt> classMembers = new List<MemberDeclarationStmt>();
            while (IsCurrentMemberDeclaration(reader) && !reader.IsAtEnd())
                classMembers.Add(MemberDeclaration(reader));

            Consume(reader, TokenType.CloseBrace, "Expected '}' at end of class declaration");

            return new ClassDecl(name, baseName, classMembers);
        }

        private static MemberDeclarationStmt MemberDeclaration(TokenReader reader)
        {
            List<Token> attributes = new List<Token>();
            while(reader.CheckCurrent(MemberAttributes) && !reader.IsAtEnd())
            {
                attributes.Add(reader.AdvanceCurrent());
            }

            if(IsCurrentDeclaration(reader))
                return new MemberDeclarationStmt(attributes, CreateDeclaration(reader));

            if (IsCurrentConstructorDeclaration(reader))
                return new MemberDeclarationStmt(attributes, ConstructorDeclaration(reader));

            ThrowError(reader, "Expected declaration.");
            return null;
        }

        private static Statement ConstructorDeclaration(TokenReader reader)
        {
            Token name = reader.AdvanceCurrent();
            List<FunctionParameter> parameters = ReadParameters(reader);

            Consume(reader, TokenType.OpenBrace, "Expect '{' after function definition.");
            BlockStmt body = (BlockStmt)Block(reader);
            return new ConstructorDecl(name, parameters, body);
        }

        private static bool IsCurrentClassDeclaration(TokenReader reader)
        {
            return reader.PeekCurrent().Type == TokenType.Class;
        }

        private static bool IsCurrentDeclaration(TokenReader reader)
        {
            return IsCurrentFunctionDeclaration(reader) ||
                   IsCurrentVariableDeclaration(reader) ||
                   IsCurrentClassDeclaration(reader);
        }

        private static bool IsCurrentMemberDeclaration(TokenReader reader)
        {
            return reader.CheckCurrent(MemberAttributes) || IsCurrentDeclaration(reader) || IsCurrentConstructorDeclaration(reader);
        }

        private static bool IsCurrentConstructorDeclaration(TokenReader reader)
        {
            return reader.PeekCurrent().Type == TokenType.Identifier && reader.PeekCurrent(1).Type == TokenType.OpenParen;
        }

        private static bool IsCurrentVariableDeclaration(TokenReader reader)
        {
            Token type = reader.PeekCurrent();
            Token name = reader.PeekCurrent(1);
            Token equals = reader.PeekCurrent(2);
            return TokenUtils.IsTokenAType(type) && name.Type == TokenType.Identifier && equals.Type == TokenType.Equal;
        }

        private static bool IsCurrentFunctionDeclaration(TokenReader reader)
        {
            Token type = reader.PeekCurrent();
            Token name = reader.PeekCurrent(1);
            Token openParen = reader.PeekCurrent(2);

            bool isReturnType = TokenUtils.IsTokenAType(type) || type.Type == TokenType.Void;

            return isReturnType && name.Type == TokenType.Identifier && openParen.Type == TokenType.OpenParen;
        }

        // ------------------------------------------------------------------------------------
        // Statement:
        // ------------------------------------------------------------------------------------
        private static Statement InstructionStatment(TokenReader reader)
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

            if (reader.MatchCurrent(TokenType.Return))
                return ReturnStatement(reader);

            if (IsCurrentDeclaration(reader))
                return CreateDeclaration(reader);

            return ExpressionStatment(reader);
        }

        private static Statement WhileLoop(TokenReader reader)
        {
            Consume(reader, TokenType.OpenParen, "Expect '(' after 'for'.");

            Expression condition = null;
            if (!reader.CheckCurrent(TokenType.Semicolon))
                condition = CreateExpression(reader);
            Consume(reader, TokenType.CloseParen, "Expect ')' after while clause");

            Statement body = InstructionStatment(reader);
            return new WhileLoopStmt(condition, body);
        }

        private static Statement ForLoop(TokenReader reader)
        {
            Consume(reader, TokenType.OpenParen, "Expect '(' after 'for'.");

            Statement initializer = null;
            if (IsCurrentVariableDeclaration(reader))
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

            Statement body = InstructionStatment(reader);

            return new ForLoopStmt(initializer, condition, increment, body);
        }

        private static Statement Block(TokenReader reader)
        {
            List<Statement> statements = new List<Statement>();

            while (!reader.CheckCurrent(TokenType.CloseBrace) && !reader.IsAtEnd())
                statements.Add(InstructionStatment(reader));

            Consume(reader, TokenType.CloseBrace, "Expect '}' after block statement");
            return new BlockStmt(statements);
        }

        private static Statement IfStatement(TokenReader reader)
        {
            Consume(reader, TokenType.OpenParen, "Expect '(' after 'if'");
            Expression condition = CreateExpression(reader);
            Consume(reader, TokenType.CloseParen, "Expect ')' after if condition");

            Statement thenBranch = InstructionStatment(reader);

            Statement elseBranch = null;
            if (reader.MatchCurrent(TokenType.Else))
                elseBranch = InstructionStatment(reader);

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private static Statement ContinueStatement(TokenReader reader)
        {
            Token token = reader.Previous();
            Consume(reader, TokenType.Semicolon, "Expected ';' after 'continue'");
            return new ContinueStmt(token);
        }

        private static Statement BreakStatement(TokenReader reader)
        {
            Token token = reader.Previous();
            Consume(reader, TokenType.Semicolon, "Expected ';' after 'break'");
            return new BreakStmt(token);
        }

        private static Statement ReturnStatement(TokenReader reader)
        {
            Token returnToken = reader.Previous();
            Expression expression = null;

            if (!reader.CheckCurrent(TokenType.Semicolon))
                expression = CreateExpression(reader);

            Consume(reader, TokenType.Semicolon, "Expect ';' after return value");
            return new ReturnStmt(returnToken, expression);
        }

        private static ExpressionStmt ExpressionStatment(TokenReader reader)
        {
            Expression expr = CreateExpression(reader);
            Consume(reader, TokenType.Semicolon, "Expect ';' after expression");
            return new ExpressionStmt(expr);
        }

        // ------------------------------------------------------------------------------------
        // Expression:
        // ------------------------------------------------------------------------------------

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

                if(expr is IdentifierExpr variable)
                {
                    Token name = variable.Name;
                    return new AssignmentExpr(name, value);
                }

                throw new ParserException("Invalid assignment target.", equal, reader.Current);
            }

            return expr;
        }

        private static Expression LogicOr(TokenReader reader) => GetBinaryExpression(reader, LogicAnd, TokenType.PipePipe); 
        private static Expression LogicAnd(TokenReader reader) => GetBinaryExpression(reader, Equality, TokenType.AmpersandAmpersand);
        private static Expression Equality(TokenReader reader) => GetBinaryExpression(reader, Comparison, TokenType.EqualEqual, TokenType.BangEqual);
        private static Expression Comparison(TokenReader reader) => GetBinaryExpression(reader, BitOr, TokenType.LessThen, TokenType.LessThenEqual, TokenType.GreaterThen, TokenType.GreaterThenEqual);
        private static Expression BitOr(TokenReader reader) => GetBinaryExpression(reader, BitXOr, TokenType.Pipe);
        private static Expression BitXOr(TokenReader reader) => GetBinaryExpression(reader, BitAnd, TokenType.Caret);
        private static Expression BitAnd(TokenReader reader) => GetBinaryExpression(reader, BitShift, TokenType.Ampersand);
        private static Expression BitShift(TokenReader reader) => GetBinaryExpression(reader, Term, TokenType.GreaterThanGreaterThan, TokenType.LessThanLessThan);
        private static Expression Term(TokenReader reader) => GetBinaryExpression(reader, Factor, TokenType.Plus, TokenType.Minus);
        private static Expression Factor(TokenReader reader) => GetBinaryExpression(reader, Unary, TokenType.Star, TokenType.Slash, TokenType.Percent);

        private static Expression GetBinaryExpression(TokenReader reader, Func<TokenReader, Expression> previouseExpr, params TokenType[] operatorTypes)
        {
            Expression expr = previouseExpr(reader);
            while(reader.MatchCurrent(operatorTypes))
            {
                Token op = reader.Previous();
                Expression right = previouseExpr(reader);
                return new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private static Expression Unary(TokenReader reader)
        {
            if(reader.MatchCurrent(TokenType.Minus, TokenType.Bang, TokenType.Tilda))
            {
                Token op = reader.Previous();
                Expression right = Unary(reader);
                return new UnaryExpr(right, op);
            }

            return Call(reader);
        }

        private static Expression Call(TokenReader reader)
        {
            Expression expr = Primary(reader);

            while (true)
            { // [while-true]
                if (reader.MatchCurrent(TokenType.OpenParen))
                {
                    expr = FinishFuncCallExpr(reader, expr);
                }
                else if (reader.MatchCurrent(TokenType.Dot))
                {
                    Token name = Consume(reader, TokenType.Identifier, "Expect property name after '.'.");
                    expr = new GetExpr(expr, name);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private static Expression FinishFuncCallExpr(TokenReader reader, Expression callee)
        {
            List<Expression> arguments = new List<Expression>();
            if (!reader.CheckCurrent(TokenType.CloseParen))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        ThrowError(reader, "Can't have more than 255 arguments.");
                    }
                    arguments.Add(CreateExpression(reader));
                } while (reader.MatchCurrent(TokenType.Comma));
            }

            Consume(reader, TokenType.CloseParen, "Expect ')' after arguments.");

            return new CallExpr(callee, arguments);
        }

        private static Expression Primary(TokenReader reader)
        {
            if(reader.MatchCurrent(TokenType.IntLiteral, TokenType.FloatLiteral, 
                                   TokenType.CharLiteral, TokenType.StringLiteral, 
                                   TokenType.True, TokenType.False, TokenType.Null,
                                   TokenType.This, TokenType.Base))
            {
                return new LiteralExpr(reader.Previous());
            }

            if(reader.MatchCurrent(TokenType.OpenParen))
            {
                Expression expr = CreateExpression(reader);
                Consume(reader, TokenType.CloseParen, "Expect ')' after expression.");
                return new GroupingExpr(expr);
            }

            if (reader.MatchCurrent(TokenType.New))
                return NewExpression(reader);

            if(reader.MatchCurrent(TokenType.Identifier))
            {
                return new IdentifierExpr(reader.Previous());
            }

            Token token = reader.PeekCurrent();

            if (reader.PeekCurrent().Type == TokenType.Invalid)
                throw new ParserException("Invalid token at line: " + token.Line, token, reader.Current);

            throw new ParserException("Invalid primary token: \"" + token.Lexeme + "\" at line " + token.Line, token, reader.Current);
        }

        private static Expression NewExpression(TokenReader reader)
        {
            Token keyword = reader.Previous();

            Token type = reader.PeekCurrent();
            if (!type.Type.IsBuiltInType() && !type.Type.IsIdentifier())
                ThrowError(reader, "Expected type name");

            reader.AdvanceCurrent();
            Consume(reader, TokenType.OpenParen, "Expected '('");
            List<Expression> arguments = new List<Expression>();
            if (!reader.CheckCurrent(TokenType.CloseParen))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        ThrowError(reader, "Can't have more than 255 arguments.");
                    }
                    arguments.Add(CreateExpression(reader));
                } while (reader.MatchCurrent(TokenType.Comma));
            }

            Consume(reader, TokenType.CloseParen, "Expect ')' after arguments.");

            return new NewExpr(keyword, type, arguments);
        }

        // ------------------------------------------------------------------------------------
        // Utilities:
        // ------------------------------------------------------------------------------------

        private static void ThrowError(TokenReader reader, string errorMessage)
        {
            throw new ParserException(errorMessage, reader.Previous(), reader.Current - 1);
        }

        private static Token Consume(TokenReader reader, TokenType type, string errorMessage)
        {
            if (reader.CheckCurrent(type))
                return reader.AdvanceCurrent();

            ThrowError(reader, errorMessage);
            return Token.Invalid;
        }

        private static void Synchronize(TokenReader reader)
        {
            Token current = reader.PeekCurrent();

            while (!reader.IsAtEnd())
            {
                if (IsCurrentDeclaration(reader))
                    return;

                if (TokenUtils.IsTokenAType(current))
                    return;

                if (IsSynchronizeableKeyword(current.Type))
                    return;

                reader.AdvanceCurrent();
            }
        }

        private static List<FunctionParameter> ReadParameters(TokenReader reader)
        {
            List<FunctionParameter> parameters = new List<FunctionParameter>();
            Consume(reader, TokenType.OpenParen, "Expected '('.");

            if (!reader.CheckCurrent(TokenType.CloseParen))
            {
                do
                {
                    if (parameters.Count >= 255)
                        throw new ParserException("Cant have more then 255 parameters.", reader.PeekCurrent(), reader.Current);

                    if (!TokenUtils.IsTokenAType(reader.PeekCurrent()))
                        throw new ParserException("Expected type name.", reader.PeekCurrent(), reader.Current);
                    Token paramType = reader.AdvanceCurrent();

                    Token paramName = Consume(reader, TokenType.Identifier, "Expected parameter name.");
                    parameters.Add(new FunctionParameter(paramType, paramName));
                }
                while (reader.MatchCurrent(TokenType.Comma));
            }

            Consume(reader, TokenType.CloseParen, "Expected ')'.");

            return parameters;
        }

        private static bool IsSynchronizeableKeyword(TokenType type)
        {
            return type == TokenType.Class ||
                   type == TokenType.For ||
                   type == TokenType.While ||
                   type == TokenType.Return ||
                   type == TokenType.Break ||
                   type == TokenType.Continue ||
                   type == TokenType.If ||
                   type == TokenType.Else;
        }
    }
}
