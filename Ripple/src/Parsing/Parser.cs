﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;
using Raucse.Extensions;
using Raucse;
using Ripple.Core;
using Ripple.Parsing.Errors;

namespace Ripple.Parsing
{
    public static class Parser
    {
        public static Result<ProgramStmt, List<ParserError>> Parse(List<Token> tokens, string path)
        {
            TokenReader reader = new TokenReader(tokens);
            List<ParserError> errors = new List<ParserError>();
            List<FileStmt> files = new List<FileStmt>();

            while (!reader.IsAtEnd())
            {
                try
                {
                    FileStmt file = ParseFile(ref reader, ref errors, path);
                    files.Add(file);
                }
                catch (ParserExeption e)
                {
                    errors.Add(e.Error);
                    reader.SyncronizeTo(TokenType.EOF);

                    if (!reader.IsAtEnd())
                        reader.Advance(); // go passed EOF token
                }
            }

            if (errors.Count > 0)
                return errors;

            return new ProgramStmt(files, path);
        }

        private static FileStmt ParseFile(ref TokenReader reader, ref List<ParserError> errors, string startPath)
        {
            List<Statement> declarations = new List<Statement>();

            while (!reader.IsAtEnd() && reader.Current().Type != TokenType.EOF)
            {
                try
                {
                    Statement statement = ParseDeclaration(ref reader, ref errors);
                    declarations.Add(statement);
                }
                catch (ParserExeption e)
                {
                    errors.Add(e.Error);
                    reader.SyncronizeTo(reader =>
                    {
                        return reader.Current().IsType(TokenType.Func) || 
                               IsVarDecl(reader) ||
                               reader.Current().IsType(TokenType.EOF);
                    });
                }
            }

            Token eof = reader.Consume(TokenType.EOF);
            string fullPath = eof.Text;
            string relativePath = fullPath.Substring(startPath.Length + 1, fullPath.Length - startPath.Length - 1);
            return new FileStmt(declarations, relativePath, eof);
        }

        private static Statement ParseDeclaration(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (TryParseFunctionDecl(ref reader, ref errors, out FuncDecl func))
                return func;
            else if (TryParseVarDecl(ref reader, out Statement statement))
                return statement;
            else if (TryParseExernalFunctionDecl(ref reader, out ExternalFuncDecl funcDecl))
                return funcDecl;

            ParserError error = new ExpectedDeclarationError(reader.CurrentLocation());
            throw new ParserExeption(error);
        }

        private static bool TryParseFunctionDecl(ref TokenReader reader, ref List<ParserError> errors, out FuncDecl funcDecl)
        {
            funcDecl = null;
            Token? unsafeToken = null;
            if (reader.PeekMatch(1, TokenType.Func) && reader.Match(TokenType.Unsafe))
                unsafeToken = reader.Previous();

            if (!reader.Match(TokenType.Func))
                return false;

            Token func = reader.Previous();
            Token name = reader.Consume(TokenType.Identifier);
            Option<GenericParameters> genericParameters = ParseGenericParameters(ref reader);
            Parameters parameters = ParseParameters(ref reader);
            Token arrow = reader.Consume(TokenType.RightThinArrow);
            TypeName returnType = ParseTypeName(ref reader);
            Option<WhereClause> whereClause = ParseWhereClause(ref reader);

            if (!TryParseBlock(ref reader, ref errors, out BlockStmt body))
            {
                ParserError error = new ExpectedFunctionBodyError(reader.CurrentLocation());
                throw new ParserExeption(error);
            }

            funcDecl = new FuncDecl(unsafeToken, func, name, genericParameters, parameters, arrow, returnType, whereClause, body);
            return true;
        }

        private static bool TryParseExernalFunctionDecl(ref TokenReader reader, out ExternalFuncDecl externalFuncDecl)
        {
            externalFuncDecl = null;
            if (!reader.Match(TokenType.Extern))
                return false;

            Token externToken = reader.Previous();
            Token stringLit = reader.Consume(TokenType.StringLiteral);
            Token funkToken = reader.Consume(TokenType.Func);
            Token funcName = reader.Consume(TokenType.Identifier);
            Parameters parameters = ParseParameters(ref reader);
            Token arrow = reader.Consume(TokenType.RightThinArrow);
            TypeName returnType = TypeNameParser.ParseTypeName(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon);

            externalFuncDecl = new ExternalFuncDecl(externToken, stringLit, funkToken, funcName, parameters, arrow, returnType, semiColon);
            return true;
        }

        private static Option<GenericParameters> ParseGenericParameters(ref TokenReader reader)
        {
            if(reader.Match(TokenType.LessThan))
            {
                Token lessThan = reader.Previous();

                List<Token> lifetimes = new List<Token>();
                while(!reader.Match(TokenType.GreaterThan))
                {
                    Token lifetime = reader.Consume(TokenType.Lifetime);
                    lifetimes.Add(lifetime);
                    if (reader.Current().Type != TokenType.GreaterThan)
                        reader.Consume(TokenType.Comma);
                }

                Token greaterThan = reader.Previous();

                return new GenericParameters(lessThan, lifetimes, greaterThan);
            }

            return new Option<GenericParameters>();
        }

        private static Option<WhereClause> ParseWhereClause(ref TokenReader reader)
        {
            if(reader.Match(TokenType.Where))
            {
                Token whereToken = reader.Previous();
                Expression expr = ParseExpression(ref reader);

                return new WhereClause(whereToken, expr);
            }

            return new Option<WhereClause>();
        }

        private static Parameters ParseParameters(ref TokenReader reader)
        {
            Token openParen = reader.Consume(TokenType.OpenParen);
            List<(TypeName, Token)> parameters = new List<(TypeName, Token)>();
            while(!reader.IsAtEnd() && !reader.Current().IsType(TokenType.CloseParen))
            {
                reader.Match(TokenType.Comma);
                TypeName typeName = ParseTypeName(ref reader);
                Token paramName = reader.Consume(TokenType.Identifier);
                parameters.Add((typeName, paramName));
            }

            Token closeParen = reader.Consume(TokenType.CloseParen);

            return new Parameters(openParen, parameters, closeParen);
        }

        private static Statement ParseStatement(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (TryParseIf(ref reader, ref errors, out Statement statement))
                return statement;
            else if (TryParseContinueAndBreak(ref reader, out statement))
                return statement;
            else if (TryParseBlock(ref reader, ref errors, out BlockStmt block))
                return block;
            else if (TryParseFor(ref reader, ref errors, out statement))
                return statement;
            else if (TryParseWhile(ref reader, ref errors, out statement))
                return statement;
            else if (TryParseReturn(ref reader, out statement))
                return statement;
            else if (TryParseVarDecl(ref reader, out statement))
                return statement;
            else if (TryParseUnsafeBlock(ref reader, ref errors, out UnsafeBlock unsafeBlock))
                return unsafeBlock;
            else
                return ParseExpressionStatement(ref reader);
        }

        private static bool TryParseContinueAndBreak(ref TokenReader reader, out Statement statement)
        {
            statement = null;

            if (reader.Match(TokenType.Break))
                statement = new BreakStmt(reader.Previous(), reader.Consume(TokenType.SemiColon));
            else if (reader.Match(TokenType.Continue))
                statement = new ContinueStmt(reader.Previous(), reader.Consume(TokenType.SemiColon));

            return statement != null;
        }

        private static bool TryParseIf(ref TokenReader reader, ref List<ParserError> errors, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.If))
                return false;

            Token ifToken = reader.Previous();
            Token openParen = reader.Consume(TokenType.OpenParen);
            Expression expr = ParseExpression(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen);
            Statement body = ParseStatement(ref reader, ref errors);

            Token? elseToken = null;
            Statement elseStatement = null;
            if(reader.Match(TokenType.Else))
            {
                elseToken = reader.Previous();
                elseStatement = ParseStatement(ref reader, ref errors);
            }

            statement = new IfStmt(ifToken, openParen, expr, closeParen, body, elseToken, elseStatement);
            return true;
        }

        private static bool TryParseFor(ref TokenReader reader, ref List<ParserError> errors, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.For))
                return false;

            Token forToken = reader.Previous();

            Token openParen = reader.Consume(TokenType.OpenParen);

            if (!TryParseVarDecl(ref reader, out Statement init))
                init = null;

            Expression condition = null;
            if (reader.Current().Type != TokenType.SemiColon)
                condition = ParseExpression(ref reader);
            reader.Consume(TokenType.SemiColon);

            Expression itr = null;
            if (reader.Current().Type != TokenType.CloseParen)
                itr = ParseExpression(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen);

            Statement body = ParseStatement(ref reader, ref errors);

            statement = new ForStmt(forToken, openParen, init, condition, itr, closeParen, body);
            return true;
        }

        private static bool TryParseWhile(ref TokenReader reader, ref List<ParserError> parserErrors, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.While))
                return false;

            Token whileToken = reader.Previous();
            Token openParen = reader.Consume(TokenType.OpenParen);
            Expression condition = ParseExpression(ref reader);
            Token closeParen = reader.Consume(TokenType.CloseParen);

            Statement body = ParseStatement(ref reader, ref parserErrors);

            statement = new WhileStmt(whileToken, openParen, condition, closeParen, body);
            return true;
        }

        private static bool TryParseUnsafeBlock(ref TokenReader reader, ref List<ParserError> errors, out UnsafeBlock unsafeBlock)
        {
            unsafeBlock = null;
            if(reader.Match(TokenType.Unsafe))
            {
                Token unsafeToken = reader.Previous();
                Token openBrace = reader.Consume(TokenType.OpenBrace);
                List<Statement> statements = new List<Statement>();

                while (!reader.Match(TokenType.CloseBrace))
                    statements.Add(ParseStatement(ref reader, ref errors));

                Token closeBrace = reader.Previous();


                unsafeBlock = new UnsafeBlock(unsafeToken, openBrace, statements, closeBrace);
                return true;
            }

            return false;
        }

        private static bool TryParseReturn(ref TokenReader reader, out Statement statement)
        {
            statement = null;
            if (!reader.Match(TokenType.Return))
                return false;

            Token returnToken = reader.Previous();
            if(reader.Match(TokenType.SemiColon))
            {
                statement = new ReturnStmt(returnToken, null, reader.Previous());
                return true;
            }

            Expression expr = ParseExpression(ref reader);

            Token semiColon = reader.Consume(TokenType.SemiColon);

            statement = new ReturnStmt(returnToken, expr, semiColon);
            return true;
        }

        private static bool TryParseVarDecl(ref TokenReader reader, out Statement statement)
        {
            statement = null;

            if (reader.Current().IsType(TokenType.Unsafe))
            {
                if (!IsVarDecl(reader, 1)) // offset by one
                    return false;
            }
            else
            {
                if (!IsVarDecl(reader))
                    return false;
            }

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            TypeName type = ParseTypeName(ref reader);
            Token? mutToken = reader.TryMatch(TokenType.Mut);
            List<Token> varNames = new List<Token>();
            varNames.Add(reader.Advance());

            while(reader.Match(TokenType.Comma))
            {
                varNames.Add(reader.Consume(TokenType.Identifier));
            }

            Token equel = reader.Consume(TokenType.Equal);

            Expression expr = ParseExpression(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon);

            statement = new VarDecl(unsafeToken, type, mutToken, varNames, equel, expr, semiColon);
            return true;
        }

        private static bool IsVarDecl(TokenReader reader, int beginOffset = 0)
        {
            if(IsTypeName(ref reader, out int length, beginOffset))
            {
                return reader.Peek(length + beginOffset) is Token t && 
                       (t.Type.IsType(TokenType.Identifier, TokenType.Mut));
            }

            return false;
        }

        private static bool TryParseBlock(ref TokenReader reader, ref List<ParserError> errors, out BlockStmt statement)
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
                        errors.Add(e.Error);
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
            Token semiColon = reader.Consume(TokenType.SemiColon);
            return new ExprStmt(expr, semiColon);
        }

        private static Expression ParseExpression(ref TokenReader reader)
        {
            return ParseAssignment(ref reader);
        }

        private static Expression ParseAssignment(ref TokenReader reader)
        {
            Expression obj = ParseCasting(ref reader);
            if(obj is AST.Index || obj is Identifier || obj is Unary u && u.Op.Type == TokenType.Star) // is an index, identifier, or dereference
            {
                if(reader.Match(TokenType.Equal))
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
            while(reader.Match(TokenType.As))
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
            if(reader.CheckSequence(TokenType.Ampersand, TokenType.Mut))
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

            if(reader.Match(TokenType.Minus, TokenType.Bang, TokenType.Star, TokenType.Ampersand))
            {
                Token tok = reader.Previous();
                Expression expr = ParseUnaryExpr(reader);
                return new Unary(tok, expr);
            }

            return ParseCallOrIndex(reader);
        }

        private static Expression ParseCallOrIndex(TokenReader reader)
        {
            Expression expr = ParsePrimary(reader);
            expr = ParseCallOrIndexArgs(reader, expr);

            return expr;
        }

        private static Expression ParseCallOrIndexArgs(TokenReader reader, Expression callee)
        {
            if(reader.Match(TokenType.OpenParen)) // call
            {
                Token openParen = reader.Previous();
                List<Expression> arguments = new List<Expression>();
                while(reader.Current().Type != TokenType.CloseParen)
                {
                    arguments.Add(ParseExpression(ref reader));

                    if (reader.Current().Type != TokenType.CloseParen)
                        reader.Consume(TokenType.Comma);
                }
                Token closeParen = reader.Consume(TokenType.CloseParen);
                Expression call = new Call(callee, openParen, arguments, closeParen);
                return ParseCallOrIndexArgs(reader, call);
            }
            else if(reader.Match(TokenType.OpenBracket))
            {
                Token openBracket = reader.Previous();
                Expression expression = ParseExpression(ref reader);
                Token closeBracket = reader.Consume(TokenType.CloseBracket);
                Expression index = new AST.Index(callee, openBracket, expression, closeBracket);
                return ParseCallOrIndexArgs(reader, index);
            }
            else
            {
                return callee;
            }
        }

        private static Expression ParsePrimary(TokenReader reader)
        {
            if (reader.Match(TokenType.IntagerLiteral, TokenType.FloatLiteral, TokenType.True, TokenType.False, TokenType.StringLiteral, TokenType.CStringLiteral, TokenType.CharactorLiteral, TokenType.Nullptr))
                return new Literal(reader.Previous());
            if (TryParseTypeExpression(ref reader, out TypeExpression typeExpression))
                return typeExpression;
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

        private static bool TryParseTypeExpression(ref TokenReader reader, out TypeExpression typeExpression)
        {
            typeExpression = null;
            if(reader.CheckSequence(TokenType.Identifier, TokenType.LessThan, TokenType.Lifetime))
            {
                Token typeName = reader.Advance();
                Token lessThan = reader.Advance();
                List<Token> lifetimes = new List<Token>();
                lifetimes.Add(reader.Advance());

                while(!reader.Match(TokenType.GreaterThan))
                {
                    reader.Consume(TokenType.Comma);
                    Token lifetime = reader.Consume(TokenType.Lifetime);
                    lifetimes.Add(lifetime);
                }

                Token greaterThan = reader.Previous();

                typeExpression = new TypeExpression(typeName, greaterThan, lifetimes, lessThan);
                return true;
            }

            return false;
        }

        private static bool TryParseSizeof(ref TokenReader reader, out Expression sizeOf)
        {
            sizeOf = null;
            if(reader.Match(TokenType.Sizeof))
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
            while(reader.Current().Type != TokenType.CloseBrace)
            {
                expressions.Add(ParseExpression(ref reader));

                if (reader.Current().Type != TokenType.CloseBrace)
                    reader.Consume(TokenType.Comma);
            }
            Token closeBrace = reader.Consume(TokenType.CloseBrace);

            initalizerList =  new InitializerList(openBrace, expressions, closeBrace);
            return true;
        }

        private static bool ParseGrouping(TokenReader reader, out Expression expr)
        {
            if(reader.Match(TokenType.OpenParen))
            {
                Token openParen = reader.Previous();
                Expression groupedExpr = ParseExpression(ref reader);

                Token closeParen = reader.Consume(TokenType.CloseParen);
                expr = new Grouping(openParen, groupedExpr, closeParen);
                return true;
            }

            expr = null;
            return false;
        }

        private static TypeName ParseTypeName(ref TokenReader reader)
        {
            return TypeNameParser.ParseTypeName(ref reader);
        }

        private static bool IsTypeName(ref TokenReader reader, out int length, int beginOffset = 0)
        {
            return TypeNameParser.IsTypeName(ref reader, out length, beginOffset);
        }
    }
}
