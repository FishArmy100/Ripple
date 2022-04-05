using System;
using System.Collections.Generic;
using System.Text;
using Ripple.Utils;
using Ripple.AST;

namespace Ripple.Parsing
{
    static class Parser
    {
        private static readonly TokenType[] MemberAttributes = { TokenType.Public, TokenType.Protected, TokenType.Private, TokenType.Virtual, TokenType.Override, TokenType.Static };

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
        // Declarations:
        // ------------------------------------------------------------------------------------
        private static Statement CreateDeclaration(TokenReader reader)
        {
            if (IsCurrentClassDeclaration(reader))
                return ClassDeclaration(reader);

            if (IsCurrentFunctionDeclaration(reader))
                return FunctionDeclaration(reader);

            if (IsCurrentVariableDeclaration(reader))
                return VariableDeclaration(reader);

            throw new ParserException("Expected declaration", reader.PeekCurrent(), reader.Current);
        }

        private static Statement VariableDeclaration(TokenReader reader)
        {
            RippleType type = ParseType(reader);
            Token name = reader.AdvanceCurrent();
            Expression initializer = null;

            if (reader.MatchCurrent(TokenType.Equal))
                initializer = CreateExpression(reader);

            Consume(reader, TokenType.Semicolon, "Expect ';' after variable declaration.");
            return new VariableDecl(type, name, initializer);
        }

        private static Statement FunctionDeclaration(TokenReader reader)
        {
            RippleType returnType = ParseType(reader);
            Token name = reader.AdvanceCurrent();

            List<FunctionParameter> parameters = ReadParameters(reader);

            Consume(reader, TokenType.OpenBrace, "Expect '{' after function definition.");
            BlockStmt body = (BlockStmt)Block(reader);
            return new FunctionDecl(returnType, name, parameters, body);
        }

        private static Statement ClassDeclaration(TokenReader reader)
        {
            bool isStatic = reader.AdvanceCurrent().Type == TokenType.Static; // gets rid of class

            if (isStatic)
                reader.AdvanceCurrent(); // if is static, can advance again passed class

            Token name = Consume(reader, TokenType.Identifier, "Expected class name.");
            Token? baseName = null;

            if (reader.MatchCurrent(TokenType.Colon))
                baseName = Consume(reader, TokenType.Identifier, "Expected identifier as base name.");

            Consume(reader, TokenType.OpenBrace, "Expected '{' after class name");

            List<MemberDeclarationStmt> classMembers = new List<MemberDeclarationStmt>();
            while (IsCurrentMemberDeclaration(reader) && !reader.IsAtEnd())
                classMembers.Add(MemberDeclaration(reader));

            Consume(reader, TokenType.CloseBrace, "Expected '}' at end of class declaration");

            return new ClassDecl(name, baseName, classMembers, isStatic);
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

            throw CreateError(reader, "Expected declaration.");
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
            TokenType classOrStatic = reader.PeekCurrent().Type;
            TokenType classType = reader.PeekCurrent(1).Type;

            if (classOrStatic == TokenType.Class)
                return true;

            return classOrStatic == TokenType.Static && classType == TokenType.Class;
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
            bool isCurrentAType = IsCurrentRippleType(reader, out int length);

            Token name = reader.PeekCurrent(length);
            return isCurrentAType && name.Type == TokenType.Identifier;
        }

        private static bool IsCurrentFunctionDeclaration(TokenReader reader)
        {
            bool isReturnType = IsCurrentRippleType(reader, out int lenght);
            Token name = reader.PeekCurrent(lenght);
            Token openParen = reader.PeekCurrent(lenght + 1);

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

            if(reader.CheckCurrent(TokenType.OpenParen))
            {
                if(IsCurrentRippleType(reader, out int length, 1) 
                   && reader.PeekCurrent(length + 1).Type == TokenType.CloseParen)
                {
                    reader.AdvanceCurrent();
                    RippleType type = ParseType(reader);
                    reader.AdvanceCurrent();
                    Expression right = Unary(reader);
                    return new CastExpr(right, type);
                }
            }

            return Call(reader);
        }

        private static Expression Call(TokenReader reader)
        {
            Expression expr = Primary(reader);

            while (true)
            {
                if (reader.MatchCurrent(TokenType.OpenParen))
                {
                    expr = FinishFuncCallExpr(reader, expr);
                }
                else if(reader.MatchCurrent(TokenType.OpenBracket))
                {
                    expr = FinishIndexCallExpr(reader, expr);
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

        private static Expression FinishIndexCallExpr(TokenReader reader, Expression indexee)
        {
            List<Expression> arguments = new List<Expression>();
            if (!reader.CheckCurrent(TokenType.CloseBracket))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        throw CreateError(reader, "Can't have more than 255 arguments.");
                    }
                    arguments.Add(CreateExpression(reader));
                } while (reader.MatchCurrent(TokenType.Comma));
            }
            else
            {
                throw CreateError(reader, "Indexer needs more than one argument.");
            }

            Consume(reader, TokenType.CloseBracket, "Expect ')' after arguments.");

            return new IndexExpr(indexee, arguments);
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
                        throw CreateError(reader, "Can't have more than 255 arguments.");
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
                                   TokenType.CharLiteral, TokenType.CharArrayLiteral, 
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

            if(reader.CurrentToken.Type.IsBuiltInType())
            {
                reader.AdvanceCurrent();
                return new IdentifierExpr(reader.Previous());
            }

            Token token = reader.PeekCurrent();

            throw new ParserException("Invalid primary token: \"" + token.Lexeme + "\" at line " + token.Line, token, reader.Current);
        }

        private static Expression NewExpression(TokenReader reader)
        {
            Token keyword = reader.Previous();

            RippleType type = ParseType(reader);

            if(reader.MatchCurrent(TokenType.OpenParen))
            {
                return ParseNewTypeExpr(reader, keyword, type);
            }
            if(reader.MatchCurrent(TokenType.OpenBracket))
            {
                return ParseNewArrayExpr(reader, keyword, type);
            }

            throw CreateError(reader, "Expected either '(' or '['");
        }

        private static Expression ParseNewArrayExpr(TokenReader reader, Token keyword, RippleType type)
        {
            List<Expression> arguments = new List<Expression>();
            if (!reader.CheckCurrent(TokenType.CloseBracket))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        throw CreateError(reader, "Can't have more than 255 arguments.");
                    }
                    arguments.Add(CreateExpression(reader));
                } while (reader.MatchCurrent(TokenType.Comma));
            }

            Consume(reader, TokenType.CloseBracket, "Expect ')' after arguments.");

            return new NewExpr(keyword, type, arguments);
        }

        private static Expression ParseNewTypeExpr(TokenReader reader, Token keyword, RippleType type)
        {
            List<Expression> arguments = new List<Expression>();
            if (!reader.CheckCurrent(TokenType.CloseParen))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        throw CreateError(reader, "Can't have more than 255 arguments.");
                    }
                    arguments.Add(CreateExpression(reader));
                } while (reader.MatchCurrent(TokenType.Comma));
            }

            Consume(reader, TokenType.CloseParen, "Expect ')' after arguments.");

            return new NewExpr(keyword, type, arguments);
        }

        // ------------------------------------------------------------------------------------
        // Types:
        // ------------------------------------------------------------------------------------

        private static RippleType ParseType(TokenReader reader)
        {
            if(IsCurrentRippleType(reader, out int length))
            {
                return ParseType(reader, reader.Current + length);
            }

            throw CreateError(reader, "Expected type name.");
        }

        private static RippleType ParseType(TokenReader reader, int endIndex)
        {
            RippleType type = ParseBasicType(reader);

            while(true)
            {
                if (reader.Current >= endIndex)
                    break;

                if (reader.MatchCurrent(TokenType.OpenParen))
                {
                    List<RippleType> funcParams = ParseFuncRefParams(reader, out bool isNullable);
                    type = new FuncPointerType(type, funcParams, isNullable);
                    continue;
                }

                if(reader.MatchCurrent(TokenType.OpenBracket))
                {
                    int dimentions = ParseArrayDimentions(reader, out bool isNullable);
                    type = new ArrayType(type, dimentions, isNullable);
                    continue;
                }

                break;
            }

            return type;
        }

        private static int ParseArrayDimentions(TokenReader reader, out bool isNullable)
        {
            int dimetions = 1;
            while (reader.MatchCurrent(TokenType.Comma))
                dimetions++;
            Consume(reader, TokenType.CloseBracket, "Expected ']'");
            Consume(reader, TokenType.Ampersand, "Expected ']&'");
            isNullable = reader.MatchCurrent(TokenType.QuestionMark);
            return dimetions;
        }

        private static List<RippleType> ParseFuncRefParams(TokenReader reader, out bool isNullable)
        {
            List<RippleType> funcParams = new List<RippleType>();
            while (!reader.CheckCurrent(TokenType.CloseParen) && !reader.IsAtEnd())
            {
                funcParams.Add(ParseType(reader));
                if (reader.CheckCurrent(TokenType.Comma))
                    reader.AdvanceCurrent();
            }

            Consume(reader, TokenType.CloseParen, "Expected ')'.");
            Consume(reader, TokenType.Ampersand, "Expected '&'.");
            isNullable = reader.MatchCurrent(TokenType.QuestionMark);
            return funcParams;
        }

        private static BasicType ParseBasicType(TokenReader reader)
        {
            if(reader.CurrentToken.IsTypeName())
            {
                Token name = reader.AdvanceCurrent();
                bool isReference = reader.MatchCurrent(TokenType.Ampersand);
                bool isNullable = reader.MatchCurrent(TokenType.QuestionMark);
                return new BasicType(name, isReference, isNullable);
            }

            throw CreateError(reader, "Expected type name.");
        }

        private static bool IsCurrentRippleType(TokenReader reader, out int length, int offset = 0)
        {
            length = 0;
            if(reader.PeekCurrent(offset).IsTypeName())
            {
                length += GetCurrentTypeLength(reader, offset);
            }
            else
            {
                return false;
            }

            while(true)
            {
                if(IsCurrentFuncRefParameters(reader, out int paramLength, offset + length))
                {
                    length += paramLength;
                    continue;
                }

                if (IsCurrentArrayParams(reader, out int arrLength, offset + length))
                {
                    length += arrLength;
                    continue;
                }

                break;
            }

            return true;
        }

        private static bool IsCurrentFuncRefParameters(TokenReader reader, out int length, int offset)
        {
            length = 0;

            if (reader.PeekCurrent(offset + length).Type != TokenType.OpenParen)
                return false;
            length++;

            while (IsCurrentRippleType(reader, out int typeLength, offset + length))
            {
                length += typeLength;
                if (reader.PeekCurrent(offset + length).Type == TokenType.Comma)
                    length++;
            }

            if (reader.PeekCurrent(offset + length).Type != TokenType.CloseParen)
                return false;
            length++;

            if (reader.PeekCurrent(offset + length).Type != TokenType.Ampersand)
                return false;
            length++;

            if (reader.PeekCurrent(offset + length).Type == TokenType.QuestionMark)
                length++;

            return true;
        }

        private static bool IsCurrentArrayParams(TokenReader reader, out int length, int offset)
        {
            length = 0;
            if (reader.PeekCurrent(offset).Type != TokenType.OpenBracket)
                return false;
            length++;

            while (reader.PeekCurrent(offset + length).Type == TokenType.Comma)
                length++;

            if (reader.PeekCurrent(offset + length).Type != TokenType.CloseBracket)
                return false;
            length++;

            if (reader.PeekCurrent(offset + length).Type != TokenType.Ampersand)
                return false;
            length++;

            if (reader.PeekCurrent(offset + length).Type == TokenType.QuestionMark)
                length++;

            return true;
        }

        private static int GetCurrentTypeLength(TokenReader reader, int offset)
        {
            int length = 1;
            if (reader.PeekCurrent(offset + length).Type == TokenType.Ampersand)
                length++;
            if (reader.PeekCurrent(offset + length).Type == TokenType.QuestionMark)
                length++;
            return length;
        }

        // ------------------------------------------------------------------------------------
        // Utilities:
        // ------------------------------------------------------------------------------------

        private static ParserException CreateError(TokenReader reader, string errorMessage)
        {
            return new ParserException(errorMessage, reader.Previous(), reader.Current - 1);
        }

        private static Token Consume(TokenReader reader, TokenType type, string errorMessage)
        {
            if (reader.CheckCurrent(type))
                return reader.AdvanceCurrent();

            throw CreateError(reader, errorMessage);
        }

        private static void Synchronize(TokenReader reader)
        {
            Token current = reader.PeekCurrent();

            while (!reader.IsAtEnd())
            {
                if (IsCurrentDeclaration(reader))
                    return;

                if (IsCurrentConstructorDeclaration(reader))
                    break;

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

                    if (!reader.PeekCurrent().IsTypeName())
                        throw new ParserException("Expected type name.", reader.PeekCurrent(), reader.Current);
                    RippleType paramType = ParseType(reader);

                    Token paramName = Consume(reader, TokenType.Identifier, "Expected parameter name.");
                    parameters.Add(new FunctionParameter(paramName, paramType));
                }
                while (reader.MatchCurrent(TokenType.Comma));
            }

            Consume(reader, TokenType.CloseParen, "Expected ')'.");

            return parameters;
        }

        private static bool IsSynchronizeableKeyword(TokenType type)
        {
            return type == TokenType.Class      ||
                   type == TokenType.For        ||
                   type == TokenType.While      ||
                   type == TokenType.Return     ||
                   type == TokenType.Break      ||
                   type == TokenType.Continue   ||
                   type == TokenType.If         ||
                   type == TokenType.Else;
        }
    }
}
