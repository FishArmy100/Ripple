using System;
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
            else if (TryParseClassDecl(ref reader, ref errors, out ClassDecl classDecl))
                return classDecl;

            ParserError error = new ExpectedDeclarationError(reader.CurrentLocation());
            throw new ParserExeption(error);
        }

        private static bool TryParseClassDecl(ref TokenReader reader, ref List<ParserError> errors, out ClassDecl classDecl)
        {
            classDecl = null;
            Token? unsafeToken = null;
            Token classToken;

            if (reader.Match(TokenType.Class))
            {
                classToken = reader.Previous();
            }
            else if (reader.CheckSequence(TokenType.Unsafe, TokenType.Class))
            {
                unsafeToken = reader.Advance();
                classToken = reader.Advance();
            }
            else
            {
                return false;
            }

            Token className = reader.Consume(TokenType.Identifier);
            Option<GenericParameters> genericParameters = ParseGenericParameters(ref reader);
            Token openBrace = reader.Consume(TokenType.OpenBrace);

            List<MemberDecl> members = new List<MemberDecl>();
            while(true)
            {
                var member = ParseMemberDecl(ref reader, ref errors);
                if (!member.HasValue())
                    break;
                members.Add(member.Value);
            }

            Token closeBrace = reader.Consume(TokenType.CloseBrace);

            classDecl = new ClassDecl(unsafeToken, classToken, className, genericParameters, openBrace, members, closeBrace);
            return true;
        }

        private static Option<MemberDecl> ParseMemberDecl(ref TokenReader reader, ref List<ParserError> errors)
        {
            Token? accessLevel = reader.TryMatch(TokenType.Public, TokenType.Private);
            if (TryParseVarDecl(ref reader, out Statement varDecl))
                return new MemberDecl(accessLevel, varDecl);

            var constructor = ParseConstructorDecl(ref reader, ref errors);
            if (constructor.HasValue())
                return new MemberDecl(accessLevel, constructor.Value);

            var destructor = ParseDestructorDecl(ref reader, ref errors);
            if (destructor.HasValue())
                return new MemberDecl(accessLevel, destructor.Value);

            var memberFunction = ParseMemberFunction(ref reader, ref errors);
            if (memberFunction.HasValue())
                return new MemberDecl(accessLevel, memberFunction.Value);

            return new Option<MemberDecl>();
        }

        private static Option<ConstructorDecl> ParseConstructorDecl(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (!reader.CheckSequence(TokenType.Identifier) && !reader.CheckSequence(TokenType.Unsafe, TokenType.Identifier))
                return new Option<ConstructorDecl>();

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            Token identifier = reader.Consume(TokenType.Identifier);
            Option<GenericParameters> genericParameters = ParseGenericParameters(ref reader);
            Parameters parameters = ParseParameters(ref reader);
            BlockStmt body = ParseFunctionBody(ref reader, ref errors);

            return new ConstructorDecl(unsafeToken, identifier, genericParameters, parameters, body);
        }

        private static Option<DestructorDecl> ParseDestructorDecl(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (!reader.CheckSequence(TokenType.Tilda) && !reader.CheckSequence(TokenType.Unsafe, TokenType.Tilda))
                return new Option<DestructorDecl>();

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            Token tilda = reader.Consume(TokenType.Tilda);
            Token identifier = reader.Consume(TokenType.Identifier);
            Token openParen = reader.Consume(TokenType.OpenParen);
            Token closeParen = reader.Consume(TokenType.CloseParen);
            BlockStmt body = ParseFunctionBody(ref reader, ref errors);

            return new DestructorDecl(unsafeToken, tilda, identifier, openParen, closeParen, body);
        }

        private static Option<MemberFunctionDecl> ParseMemberFunction(ref TokenReader reader, ref List<ParserError> errors)
        {
            Token funcToken;
            Token? unsafeToken = null;
            if (reader.Match(TokenType.Func))
            {
                funcToken = reader.Previous();
            }
            else if (reader.CheckSequence(TokenType.Unsafe, TokenType.Func))
            {
                unsafeToken = reader.Advance();
                funcToken = reader.Advance();
            }
            else
            {
                return new Option<MemberFunctionDecl>();
            }

            Token nameToken = reader.Consume(TokenType.Identifier);
            Option<GenericParameters> genericParams = ParseGenericParameters(ref reader);
            MemberFunctionParameters parameters = ParseMemberFunctionParameters(ref reader);
            Token arrow = reader.Consume(TokenType.RightThinArrow);
            TypeName returned = TypeNameParser.ParseTypeName(ref reader);

            BlockStmt body = ParseFunctionBody(ref reader, ref errors);

            return new MemberFunctionDecl(unsafeToken, funcToken, nameToken, genericParams, parameters, arrow, returned, body);
        }

        private static MemberFunctionParameters ParseMemberFunctionParameters(ref TokenReader reader)
        {
            Token openParen = reader.Consume(TokenType.OpenParen);
            Option<ThisFunctionParameter> thisParam = ParseThisFunctionParameter(ref reader);
            List<Pair<TypeName, Token>> parameters = new List<Pair<TypeName, Token>>();

            if(thisParam.HasValue())
            {
                while(!reader.IsAtEnd() && reader.Match(TokenType.Comma)) // need a comma to start
                {
                    TypeName type = TypeNameParser.ParseTypeName(ref reader);
                    Token identifier = reader.Consume(TokenType.Identifier);
                    parameters.Add(new Pair<TypeName, Token>(type, identifier));
                }
            }
            else
            {
                while (!reader.IsAtEnd() && !reader.Current().IsType(TokenType.CloseParen)) // does not need a comma to start
                {
                    reader.Match(TokenType.Comma);
                    TypeName typeName = TypeNameParser.ParseTypeName(ref reader);
                    Token paramName = reader.Consume(TokenType.Identifier);
                    parameters.Add((typeName, paramName));
                }
            }

            Token closeParen = reader.Consume(TokenType.CloseParen);
            return new MemberFunctionParameters(openParen, thisParam, parameters, closeParen);
        }

        private static Option<ThisFunctionParameter> ParseThisFunctionParameter(ref TokenReader reader)
        {
            if(reader.Match(TokenType.This))
            {
                Token thisToken = reader.Previous();

                if(reader.Match(TokenType.Mut))
                {
                    Token mutToken = reader.Previous();
                    Token refToken = reader.Consume(TokenType.Ampersand);
                    Token? lifetimeToken = reader.TryMatch(TokenType.Lifetime);

                    return new ThisFunctionParameter(thisToken, mutToken, refToken, lifetimeToken);
                }

                if(reader.Match(TokenType.Ampersand))
                {
                    Token refToken = reader.Previous();
                    Token? lifetimeToken = reader.TryMatch(TokenType.Lifetime);
                    return new ThisFunctionParameter(thisToken, null, refToken, lifetimeToken);
                }

                return new ThisFunctionParameter(thisToken, null, null, null);
            }

            return new Option<ThisFunctionParameter>();
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
            TypeName returnType = TypeNameParser.ParseTypeName(ref reader);
            Option<WhereClause> whereClause = ParseWhereClause(ref reader);

            BlockStmt body = ParseFunctionBody(ref reader, ref errors);

            funcDecl = new FuncDecl(unsafeToken, func, name, genericParameters, parameters, arrow, returnType, whereClause, body);
            return true;
        }

        private static BlockStmt ParseFunctionBody(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (!TryParseBlock(ref reader, ref errors, out BlockStmt body))
            {
                ParserError error = new ExpectedFunctionBodyError(reader.CurrentLocation());
                throw new ParserExeption(error);
            }

            return body;
        }

        private static bool TryParseExernalFunctionDecl(ref TokenReader reader, out ExternalFuncDecl externalFuncDecl)
        {
            externalFuncDecl = null;
            Token? unsafeToken = null;

            Token? externToken;
            if (reader.Match(TokenType.Extern))
            {
                externToken = reader.Previous();
            }
            else if (reader.CheckSequence(TokenType.Unsafe, TokenType.Extern))
            {
                unsafeToken = reader.Advance();
                externToken = reader.Advance();
            }
            else
            {
                return false;
            }

            Token stringLit = reader.Consume(TokenType.StringLiteral);
            Token funkToken = reader.Consume(TokenType.Func);
            Token funcName = reader.Consume(TokenType.Identifier);
            Parameters parameters = ParseParameters(ref reader);
            Token arrow = reader.Consume(TokenType.RightThinArrow);
            TypeName returnType = TypeNameParser.ParseTypeName(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon);

            externalFuncDecl = new ExternalFuncDecl(unsafeToken, externToken.Value, stringLit, funkToken, funcName, parameters, arrow, returnType, semiColon);
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
                Expression expr = ExpressionParser.ParseExpressionOrThrow(ref reader);

                return new WhereClause(whereToken, expr);
            }

            return new Option<WhereClause>();
        }

        private static Parameters ParseParameters(ref TokenReader reader)
        {
            Token openParen = reader.Consume(TokenType.OpenParen);
            List<Pair<TypeName, Token>> parameters = new List<Pair<TypeName, Token>>();
            while(!reader.IsAtEnd() && !reader.Current().IsType(TokenType.CloseParen))
            {
                reader.Match(TokenType.Comma);
                TypeName typeName = TypeNameParser.ParseTypeName(ref reader);
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
            Expression expr = ExpressionParser.ParseExpressionOrThrow(ref reader);
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
                condition = ExpressionParser.ParseExpressionOrThrow(ref reader);
            reader.Consume(TokenType.SemiColon);

            Expression itr = null;
            if (reader.Current().Type != TokenType.CloseParen)
                itr = ExpressionParser.ParseExpressionOrThrow(ref reader);
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
            Expression condition = ExpressionParser.ParseExpressionOrThrow(ref reader);
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

            Expression expr = ExpressionParser.ParseExpressionOrThrow(ref reader);

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
            TypeName type = TypeNameParser.ParseTypeName(ref reader);
            Token? mutToken = reader.TryMatch(TokenType.Mut);
            List<Token> varNames = new List<Token>();
            varNames.Add(reader.Advance());

            while(reader.Match(TokenType.Comma))
            {
                varNames.Add(reader.Consume(TokenType.Identifier));
            }

            if(reader.Match(TokenType.SemiColon))
            {
                statement = new VarDecl(unsafeToken, type, mutToken, varNames, null, null, reader.Previous());
                return true;
            }
            else
            {
                Token equel = reader.Consume(TokenType.Equal);

                Expression expr = ExpressionParser.ParseExpressionOrThrow(ref reader);
                Token semiColon = reader.Consume(TokenType.SemiColon);

                statement = new VarDecl(unsafeToken, type, mutToken, varNames, equel, expr, semiColon);
                return true;
            }
        }

        private static bool IsVarDecl(TokenReader reader, int beginOffset = 0)
        {
            if(TypeNameParser.IsTypeName(ref reader, out int length, beginOffset))
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
            Expression expr = ExpressionParser.ParseExpressionOrThrow(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon);
            return new ExprStmt(expr, semiColon);
        }
    }
}
