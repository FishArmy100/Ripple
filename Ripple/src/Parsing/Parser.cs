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
                var file = ParseFile(reader, path);
                file.Match(
                    ok => files.Add(ok),
                    error =>
                    {
                        errors.AddRange(error);
                        reader.SyncronizeToAndAdvance(TokenType.EOF);
                    });
            }

            if (errors.Count > 0)
                return errors;

            return new ProgramStmt(files, path);
        }

        private static Result<FileStmt, List<ParserError>> ParseFile(TokenReader reader, string startPath)
        {
            List<Statement> declarations = new List<Statement>();
            List<ParserError> errors = new List<ParserError>();

            while (!reader.IsAtEnd() && reader.Current().Type != TokenType.EOF)
            {
                var statement = ParseDeclaration(reader);
                statement.Match(
                    ok => declarations.Add(ok),
                    error =>
                    {
                        errors.AddRange(error);
                        reader.SyncronizeTo(reader =>
                        {
                            return IsClassDecl(reader) || 
                                   IsFunctionDecl(reader, 0) ||
                                   IsVarDecl(reader) ||
                                   reader.Current().IsType(TokenType.EOF);
                        });
                    });
            }

            var eof = reader.Consume(TokenType.EOF);
            if (eof.IsError())
                errors.Add(eof.Error);

            if (errors.Any())
                return errors;

            string fullPath = eof.Value.Text;
            string relativePath = fullPath.Substring(startPath.Length + 1, fullPath.Length - startPath.Length - 1);
            return new FileStmt(declarations, relativePath, eof.Value);
        }

        private static Result<Statement, List<ParserError>> ParseDeclaration(TokenReader reader)
        {
            var varDecl = TryParseVarDecl(ref reader);
            if(varDecl.HasValue())
            {
                return varDecl.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    error => new List<ParserError> { error });
            }

            var funcDecl = TryParseFunctionDecl(reader);
            if (funcDecl.HasValue())
            {
                return funcDecl.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok),
                    error => error);
            }

            var externFuncDecl = TryParseExernalFunctionDecl(ref reader);
            if (externFuncDecl.HasValue())
            {
                return externFuncDecl.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok),
                    error => new List<ParserError> { error });
            }

            var classDecl = TryParseClassDecl(reader);
            if (classDecl.HasValue())
            {
                return classDecl.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok),
                    error => error);
            }

            var externClassDecl = TryParseExternClassDecl(reader);
            if(externClassDecl.HasValue())
            {
                return externClassDecl.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    error => error);
            }

            ParserError error = new ExpectedDeclarationError(reader.CurrentLocation());
            return new List<ParserError> { error };
        }

        private static Option<Result<ClassDecl, List<ParserError>>> TryParseClassDecl(TokenReader reader)
        {
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
                return new Option<Result<ClassDecl, List<ParserError>>>();
            }

            List<ParserError> errors = new List<ParserError>();

            var className = reader.Consume(TokenType.Identifier);
            if(className.IsError())
            {
                errors.Add(className.Error);
                reader.SyncronizeTo(TokenType.OpenBrace);
            }


            var genericParameters = ParseGenericParameters(ref reader).Match(
                some =>
                {
                    return some.Match(
                        ok => ok,
                        error =>
                        {
                            errors.Add(error);
                            reader.SyncronizeTo(TokenType.OpenBrace);
                            return new Option<GenericParameters>();
                        });
                },
                () => new Option<GenericParameters>());


            var openBrace = reader.Consume(TokenType.OpenBrace);
            if(openBrace.IsError())
            {
                errors.Add(openBrace.Error);
                return new Option<Result<ClassDecl, List<ParserError>>>(errors);
            }

            List<MemberDecl> members = new List<MemberDecl>();
            while(true)
            {
                var member = TryParseMemberDecl(ref reader);
                if (!member.HasValue())
                    break;

                member.Value.Match(
                    ok => members.Add(ok),
                    error =>
                    {
                        errors.AddRange(error);
                        reader.SyncronizeTo(reader =>
                        {
                            return IsMemberDecl(reader) || reader.CurrentIs(TokenType.CloseBrace);
                        });
                    });
            }

            var closeBrace = reader.Consume(TokenType.CloseBrace);
            if (closeBrace.IsError())
                errors.Add(closeBrace.Error);

            if (errors.Any())
                return new Option<Result<ClassDecl, List<ParserError>>>(errors);

            ClassDecl classDecl = new ClassDecl(unsafeToken, classToken, className.Value, genericParameters, openBrace.Value, members, closeBrace.Value);
            return new Option<Result<ClassDecl, List<ParserError>>>(classDecl);
        }

        private static Option<Result<MemberDecl, List<ParserError>>> TryParseMemberDecl(ref TokenReader reader)
        {
            Token? accessLevel = reader.TryMatch(TokenType.Public, TokenType.Private);
            var varDecl = TryParseVarDecl(ref reader);
            if(varDecl.HasValue())
            {
                return varDecl.Value.Match(
                    ok =>
                    {
                        MemberDecl member = new MemberDecl(accessLevel, ok);
                        return new Option<Result<MemberDecl, List<ParserError>>>(member);
                    },
                    error =>
                    {
                        return new Option<Result<MemberDecl, List<ParserError>>>(new Result<MemberDecl, List<ParserError>>(
                            new List<ParserError> 
                            { 
                                error 
                            }));
                    });
            }

            var constructor = TryParseConstructorDecl(reader);
            if (constructor.HasValue())
            {
                return constructor.Value.Match(
                    ok =>
                    {
                        MemberDecl member = new MemberDecl(accessLevel, ok);
                        return new Option<Result<MemberDecl, List<ParserError>>>(member);
                    },
                    error =>
                    {
                        return new Option<Result<MemberDecl, List<ParserError>>>(error);
                    });
            }

            var destructor = TryParseDestructorDecl(reader);
            if (destructor.HasValue())
            {
                return destructor.Value.Match(
                    ok =>
                    {
                        MemberDecl member = new MemberDecl(accessLevel, ok);
                        return new Option<Result<MemberDecl, List<ParserError>>>(member);
                    },
                    error =>
                    {
                        return new Option<Result<MemberDecl, List<ParserError>>>(error);
                    });
            }

            var memberFunction = TryParseMemberFunction(reader);
            if (memberFunction.HasValue())
            {
                return memberFunction.Value.Match(
                    ok =>
                    {
                        MemberDecl member = new MemberDecl(accessLevel, ok);
                        return new Option<Result<MemberDecl, List<ParserError>>>(member);
                    },
                    error =>
                    {
                        return new Option<Result<MemberDecl, List<ParserError>>>(error);
                    });
            }

            return new Option<Result<MemberDecl, List<ParserError>>>();
        }

        private static Option<Result<ConstructorDecl, List<ParserError>>> TryParseConstructorDecl(TokenReader reader)
        {
            if (!reader.CheckSequence(TokenType.Identifier) && !reader.CheckSequence(TokenType.Unsafe, TokenType.Identifier))
                return new Option<Result<ConstructorDecl, List<ParserError>>>();

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            Token identifier = reader.Advance();

            List<ParserError> errors = new List<ParserError>();

            var genericParameters = ParseGenericParameters(ref reader).Match(
                some =>
                {
                    return some.Match(
                        ok => ok,
                        error =>
                        {
                            errors.Add(error);
                            reader.SyncronizeTo(TokenType.OpenParen);
                            return new Option<GenericParameters>();
                        });
                },
                () => new Option<GenericParameters>());

            var parameters = ParseParameters(ref reader);
            if (parameters.IsError())
            {
                reader.SyncronizeTo(TokenType.RightThinArrow, TokenType.Where, TokenType.OpenBrace);
                errors.Add(parameters.Error);
            }

            var body = ParseFunctionBody(reader);
            if (body.IsError())
                errors.AddRange(body.Error);

            if (errors.Any())
                return new Option<Result<ConstructorDecl, List<ParserError>>>(errors);

            ConstructorDecl constructorDecl = new ConstructorDecl(unsafeToken, identifier, genericParameters, parameters.Value, body.Value);
            return new Option<Result<ConstructorDecl, List<ParserError>>>(constructorDecl);
        }

        private static Option<Result<DestructorDecl, List<ParserError>>> TryParseDestructorDecl(TokenReader reader)
        {
            if (!reader.CheckSequence(TokenType.Tilda) && !reader.CheckSequence(TokenType.Unsafe, TokenType.Tilda))
                return new Option<Result<DestructorDecl, List<ParserError>>>();

            List<ParserError> errors = new List<ParserError>();

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            Token tilda = reader.Advance();
            var identifier = reader.Consume(TokenType.Identifier);
            if (identifier.IsError())
            {
                errors.Add(identifier.Error);
                reader.SyncronizeTo(TokenType.OpenParen, TokenType.CloseParen, TokenType.OpenBrace);
            }

            var openParen = reader.Consume(TokenType.OpenParen);
            if(openParen.IsError())
            {
                errors.Add(openParen.Error);
                reader.SyncronizeTo(TokenType.CloseParen, TokenType.OpenBrace);
            }

            var closeParen = reader.Consume(TokenType.CloseParen);
            if(closeParen.IsError())
            {
                errors.Add(closeParen.Error);
                reader.SyncronizeTo(TokenType.OpenBrace);
            }

            var body = ParseFunctionBody(reader);
            if (body.IsError())
                errors.AddRange(body.Error);

            if (errors.Any())
                return new Option<Result<DestructorDecl, List<ParserError>>>(errors);

            DestructorDecl destructorDecl = new DestructorDecl(unsafeToken, tilda, identifier.Value, openParen.Value, closeParen.Value, body.Value);
            return new Option<Result<DestructorDecl, List<ParserError>>>(destructorDecl);
        }

        private static Option<Result<MemberFunctionDecl, List<ParserError>>> TryParseMemberFunction(TokenReader reader)
        {
            Token? unsafeToken = null;
            if (reader.PeekMatch(1, TokenType.Func) && reader.Match(TokenType.Unsafe))
                unsafeToken = reader.Previous();

            if (!reader.Match(TokenType.Func))
                return new Option<Result<MemberFunctionDecl, List<ParserError>>>();

            List<ParserError> errors = new List<ParserError>();

            var func = reader.Previous();
            var name = reader.Consume(TokenType.Identifier);
            if (name.IsError())
                errors.Add(name.Error);

            var genericParameters = ParseGenericParameters(ref reader).Match(
                some =>
                {
                    return some.Match(
                        ok => ok,
                        error =>
                        {
                            errors.Add(error);
                            reader.SyncronizeTo(TokenType.OpenParen);
                            return new Option<GenericParameters>();
                        });
                },
                () => new Option<GenericParameters>());

            var parameters = ParseMemberFunctionParameters(ref reader);
            if (parameters.IsError())
            {
                reader.SyncronizeTo(TokenType.RightThinArrow, TokenType.Where, TokenType.OpenBrace);
                errors.Add(parameters.Error);
            }

            var arrow = reader.Consume(TokenType.RightThinArrow);
            if (arrow.IsError())
                errors.Add(arrow.Error);

            var returnType = TypeNameParser.ParseTypeName(ref reader);
            if (returnType.IsError())
            {
                errors.Add(returnType.Error);
                reader.SyncronizeTo(TokenType.OpenBrace, TokenType.Where);
            }

            var whereClause = ParseWhereClause(ref reader).Match(
                some =>
                {
                    return some.Match(
                        ok => ok,
                        error =>
                        {
                            errors.Add(error);
                            reader.SyncronizeTo(TokenType.OpenBrace);
                            return new Option<WhereClause>();
                        });
                },
                () => new Option<WhereClause>());


            var body = ParseFunctionBody(reader);
            if (body.IsError())
                errors.AddRange(body.Error);

            if (errors.Any())
                return new Option<Result<MemberFunctionDecl, List<ParserError>>>(errors);

            MemberFunctionDecl funcDecl = new MemberFunctionDecl(unsafeToken, func, name.Value, genericParameters, parameters.Value, arrow.Value, returnType.Value, whereClause, body.Value);
            return new Option<Result<MemberFunctionDecl, List<ParserError>>>(funcDecl);
        }

        private static Result<MemberFunctionParameters, ParserError> ParseMemberFunctionParameters(ref TokenReader reader)
        {
            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return openParen.Error;

            var thisParamResult = ParseThisFunctionParameter(ref reader);
            if (thisParamResult.HasValue() && thisParamResult.Value.IsError())
                return thisParamResult.Value.Error;

            var thisParam = thisParamResult.Match(
                ok => new Option<ThisFunctionParameter>(ok.Value), 
                () => new Option<ThisFunctionParameter>());

            List<Pair<TypeName, Token>> parameters = new List<Pair<TypeName, Token>>();

            if(thisParam.HasValue())
            {
                while(!reader.IsAtEnd() && reader.Match(TokenType.Comma)) // need a comma to start
                {
                    var type = TypeNameParser.ParseTypeName(ref reader);
                    if (type.IsError())
                        return type.Error;

                    var identifier = reader.Consume(TokenType.Identifier);
                    if (identifier.IsError())
                        return identifier.Error;

                    parameters.Add(new Pair<TypeName, Token>(type.Value, identifier.Value));
                }
            }
            else
            {
                while (!reader.IsAtEnd() && !reader.Current().IsType(TokenType.CloseParen)) // does not need a comma to start
                {
                    reader.Match(TokenType.Comma);
                    var typeName = TypeNameParser.ParseTypeName(ref reader);
                    if (typeName.IsError())
                        return typeName.Error;

                    var paramName = reader.Consume(TokenType.Identifier);
                    if (paramName.IsError())
                        return paramName.Error;

                    parameters.Add((typeName.Value, paramName.Value));
                }
            }

            var closeParen = reader.Consume(TokenType.CloseParen);
            if (closeParen.IsError())
                return closeParen.Error;

            return new MemberFunctionParameters(openParen.Value, thisParam, parameters, closeParen.Value);
        }

        private static Option<Result<ThisFunctionParameter, ParserError>> ParseThisFunctionParameter(ref TokenReader reader)
        {
            if(reader.Match(TokenType.This))
            {
                Token thisToken = reader.Previous();

                if(reader.Match(TokenType.Mut))
                {
                    Token mutToken = reader.Previous();
                    var refToken = reader.Consume(TokenType.Ampersand);
                    if (refToken.IsError())
                        return new Option<Result<ThisFunctionParameter, ParserError>>(refToken.Error);

                    Token? lifetimeToken = reader.TryMatch(TokenType.Lifetime);

                    return new(new ThisFunctionParameter(thisToken, mutToken, refToken.Value, lifetimeToken));
                }

                if(reader.Match(TokenType.Ampersand))
                {
                    Token refToken = reader.Previous();
                    Token? lifetimeToken = reader.TryMatch(TokenType.Lifetime);
                    return new (new ThisFunctionParameter(thisToken, null, refToken, lifetimeToken));
                }

                return new (new ThisFunctionParameter(thisToken, null, null, null));
            }

            return new Option<Result<ThisFunctionParameter, ParserError>>();
        }

        private static Option<Result<ExternClassDecl, List<ParserError>>> TryParseExternClassDecl(TokenReader reader)
        {
            bool isExternClassDecl = reader.CheckSequence(TokenType.Unsafe, TokenType.Extern, TokenType.StringLiteral, TokenType.Class) ||
                                     reader.CheckSequence(TokenType.Extern, TokenType.StringLiteral, TokenType.Class);

            if (!isExternClassDecl)
                return new Option<Result<ExternClassDecl, List<ParserError>>>();

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            Token externToken = reader.Advance();
            Token specifier = reader.Advance();
            Token classToken = reader.Advance();

            var nameToken = reader.Consume(TokenType.Identifier);
            if (nameToken.IsError())
                return new Option<Result<ExternClassDecl, List<ParserError>>>(new List<ParserError> { nameToken.Error });

            var openBrace = reader.Consume(TokenType.OpenBrace);
            if (openBrace.IsError())
                return new Option<Result<ExternClassDecl, List<ParserError>>>(new List<ParserError> { openBrace.Error });


            List<ParserError> errors = new List<ParserError>();
            List<ExternClassMemberDecl> members = new List<ExternClassMemberDecl>();

            while(true)
            {
                var member = TryParseExternClassMemberDecl(reader);
                if (member.HasValue())
                {
                    if (member.Value.IsError())
                    {
                        errors.Add(member.Value.Error);
                    }
                    else
                    {
                        members.Add(member.Value.Value);
                        reader.SyncronizeTo(TokenType.Public, TokenType.Private, TokenType.CloseBrace);
                    }
                }
                else break;
            }

            var closeBrace = reader.Consume(TokenType.CloseBrace);
            if (closeBrace.IsError())
                errors.Add(closeBrace.Error);

            if (errors.Any())
                return new Option<Result<ExternClassDecl, List<ParserError>>>(errors);

            ExternClassDecl externClassDecl = new ExternClassDecl(unsafeToken, externToken, specifier, classToken, nameToken.Value, openBrace.Value, members, closeBrace.Value);
            return new Option<Result<ExternClassDecl, List<ParserError>>>(externClassDecl);
        }

        private static Option<Result<ExternClassMemberDecl, ParserError>> TryParseExternClassMemberDecl(TokenReader reader)
        {
            if (!reader.Match(TokenType.Public, TokenType.Private))
                return new Option<Result<ExternClassMemberDecl, ParserError>>();

            Token visibility = reader.Previous();
            var type = TypeNameParser.ParseTypeName(ref reader);
            if (type.IsError())
                return new Option<Result<ExternClassMemberDecl, ParserError>>(type.Error);

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);

            var name = reader.Consume(TokenType.Identifier);
            if (name.IsError())
                return new Option<Result<ExternClassMemberDecl, ParserError>>(name.Error);

            var semiColon = reader.Consume(TokenType.SemiColon);
            if (semiColon.IsError())
                return new Option<Result<ExternClassMemberDecl, ParserError>>(semiColon.Error);

            ExternClassMemberDecl member = new ExternClassMemberDecl(visibility, unsafeToken, type.Value, name.Value, semiColon.Value);
            return new Option<Result<ExternClassMemberDecl, ParserError>>(member);
        }

        private static Option<Result<FuncDecl, List<ParserError>>> TryParseFunctionDecl(TokenReader reader)
        {
            Token? unsafeToken = null;
            if (reader.PeekMatch(1, TokenType.Func) && reader.Match(TokenType.Unsafe))
                unsafeToken = reader.Previous();

            if (!reader.Match(TokenType.Func))
                return new Option<Result<FuncDecl, List<ParserError>>>();

            List<ParserError> errors = new List<ParserError>();

            var func = reader.Previous();
            var name = reader.Consume(TokenType.Identifier);
            if (name.IsError())
                errors.Add(name.Error);

            var genericParameters = ParseGenericParameters(ref reader).Match(
                some =>
                {
                    return some.Match(
                        ok => ok,
                        error =>
                        {
                            errors.Add(error);
                            reader.SyncronizeTo(TokenType.OpenParen);
                            return new Option<GenericParameters>();
                        });
                },
                () => new Option<GenericParameters>());

            var parameters = ParseParameters(ref reader);
            if(parameters.IsError())
            {
                reader.SyncronizeTo(TokenType.RightThinArrow, TokenType.Where, TokenType.OpenBrace);
                errors.Add(parameters.Error);
            }

            var arrow = reader.Consume(TokenType.RightThinArrow);
            if (arrow.IsError())
                errors.Add(arrow.Error);

            var returnType = TypeNameParser.ParseTypeName(ref reader);
            if(returnType.IsError())
            {
                errors.Add(returnType.Error);
                reader.SyncronizeTo(TokenType.OpenBrace, TokenType.Where);
            }

            var whereClause = ParseWhereClause(ref reader).Match(
                some =>
                {
                    return some.Match(
                        ok => ok,
                        error =>
                        {
                            errors.Add(error);
                            reader.SyncronizeTo(TokenType.OpenBrace);
                            return new Option<WhereClause>();
                        });
                },
                () => new Option<WhereClause>());


            var body = ParseFunctionBody(reader);
            if (body.IsError())
                errors.AddRange(body.Error);

            if (errors.Any())
                return new Option<Result<FuncDecl, List<ParserError>>>(errors);

            FuncDecl funcDecl = new FuncDecl(unsafeToken, func, name.Value, genericParameters, parameters.Value, arrow.Value, returnType.Value, whereClause, body.Value);
            return new Option<Result<FuncDecl, List<ParserError>>>(funcDecl);
        }

        private static Result<BlockStmt, List<ParserError>> ParseFunctionBody(TokenReader reader)
        {
            return TryParseBlock(reader).Match(body =>
            {
                return body.Match(
                    ok => ok,
                    errors => new Result<BlockStmt, List<ParserError>>(errors));
            },
            () =>
            {
                ParserError error = new ExpectedFunctionBodyError(reader.CurrentLocation());
                return new Result<BlockStmt, List<ParserError>>(new List<ParserError> { error });
            });
        }

        private static Option<Result<ExternalFuncDecl, ParserError>> TryParseExernalFunctionDecl(ref TokenReader reader)
        {
            bool isExternFunc = reader.CheckSequence(TokenType.Unsafe, TokenType.Extern, TokenType.StringLiteral, TokenType.Func) ||
                                reader.CheckSequence(TokenType.Extern, TokenType.StringLiteral, TokenType.Func);

            if (!isExternFunc)
                return new Option<Result<ExternalFuncDecl, ParserError>>();

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            Token externToken = reader.Advance();

            var stringLit = reader.Consume(TokenType.StringLiteral);
            if (stringLit.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(stringLit.Error);

            var funcToken = reader.Consume(TokenType.Func);
            if (funcToken.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(funcToken.Error);

            var funcName = reader.Consume(TokenType.Identifier);
            if (funcName.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(funcToken.Error);

            var parameters = ParseParameters(ref reader);
            if (parameters.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(parameters.Error);

            var arrow = reader.Consume(TokenType.RightThinArrow);
            if (arrow.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(arrow.Error);

            var returnType = TypeNameParser.ParseTypeName(ref reader);
            if (returnType.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(returnType.Error);

            var semiColon = reader.Consume(TokenType.SemiColon);
            if (semiColon.IsError())
                return new Option<Result<ExternalFuncDecl, ParserError>>(semiColon.Error);

            ExternalFuncDecl externalFuncDecl = new ExternalFuncDecl(unsafeToken, externToken, stringLit.Value, funcToken.Value, funcName.Value, parameters.Value, arrow.Value, returnType.Value, semiColon.Value);
            return new Option<Result<ExternalFuncDecl, ParserError>>(externalFuncDecl);
        }

        private static Option<Result<GenericParameters, ParserError>> ParseGenericParameters(ref TokenReader reader)
        {
            if(reader.Match(TokenType.LessThan))
            {
                Token lessThan = reader.Previous();

                List<Token> lifetimes = new List<Token>();
                while(!reader.Match(TokenType.GreaterThan))
                {
                    var lifetime = reader.Consume(TokenType.Lifetime);
                    if (lifetime.IsError())
                        return new Option<Result<GenericParameters, ParserError>>(lifetime.Error);

                    lifetimes.Add(lifetime.Value);
                    if (reader.Current().Type != TokenType.GreaterThan)
                        reader.Consume(TokenType.Comma);
                }

                Token greaterThan = reader.Previous();

                GenericParameters genericParameters = new GenericParameters(lessThan, lifetimes, greaterThan);
                return new Option<Result<GenericParameters, ParserError>>(genericParameters);
            }

            return new Option<Result<GenericParameters, ParserError>>();
        }

        private static Option<Result<WhereClause, ParserError>> ParseWhereClause(ref TokenReader reader)
        {
            if(reader.Match(TokenType.Where))
            {
                Token whereToken = reader.Previous();
                var expr = ExpressionParser.ParseExpression(ref reader);
                if (expr.IsError())
                    return new Option<Result<WhereClause, ParserError>>(expr.Error);

                WhereClause whereClause = new WhereClause(whereToken, expr.Value);
                return new Option<Result<WhereClause, ParserError>>(whereClause);
            }

            return new Option<Result<WhereClause, ParserError>>();
        }

        private static Result<Parameters, ParserError> ParseParameters(ref TokenReader reader)
        {
            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return openParen.Error;

            List<Pair<TypeName, Token>> parameters = new List<Pair<TypeName, Token>>();
            while(!reader.IsAtEnd() && !reader.Current().IsType(TokenType.CloseParen))
            {
                reader.Match(TokenType.Comma);
                var typeName = TypeNameParser.ParseTypeName(ref reader);
                if (typeName.IsError())
                    return typeName.Error;

                var paramName = reader.Consume(TokenType.Identifier);
                if (paramName.IsError())
                    return paramName.Error;

                parameters.Add((typeName.Value, paramName.Value));
            }

            var closeParen = reader.Consume(TokenType.CloseParen);
            if (closeParen.IsError())
                return closeParen.Error;

            return new Parameters(openParen.Value, parameters, closeParen.Value);
        }

        private static Result<Statement, List<ParserError>> ParseStatement(ref TokenReader reader)
        {
            var ifStmt = TryParseIf(ref reader);
            if (ifStmt.HasValue())
            {
                return ifStmt.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    fail => fail);
            }

            var continueOrBreak = TryParseContinueAndBreak(reader);
            if (continueOrBreak.HasValue())
            {
                return continueOrBreak.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok),
                    fail => new List<ParserError> { fail });
            }

            var block = TryParseBlock(reader);
            if(block.HasValue())
            {
                return block.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    fail => fail);
            }

            var forStmt = TryParseFor(reader);
            if(forStmt.HasValue())
            {
                return forStmt.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    fail => fail);
            }

            var whileStmt = TryParseWhile(ref reader);
            if (whileStmt.HasValue())
            {
                return whileStmt.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    fail => fail);
            }

            var returnStmt = TryParseReturn(ref reader);
            if (returnStmt.HasValue())
            {
                return returnStmt.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok),
                    fail => new List<ParserError> { fail });
            }

            var varDecl = TryParseVarDecl(ref reader);
            if(varDecl.HasValue())
            {
                return varDecl.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    fail => new List<ParserError> { fail });
            }

            var unsafeBlock = TryParseUnsafeBlock(reader);
            if (unsafeBlock.HasValue())
            {
                return unsafeBlock.Value.Match(
                    ok => new Result<Statement, List<ParserError>>(ok), 
                    fail => fail);
            }

            return ParseExpressionStatement(ref reader).Match(
                ok => new Result<Statement, List<ParserError>>(ok), 
                fail => new List<ParserError> { fail });
        }

        private static Option<Result<Statement, ParserError>> TryParseContinueAndBreak(TokenReader reader)
        {
            if (reader.Match(TokenType.Break))
            {
                Token breakToken = reader.Previous();
                return reader.Consume(TokenType.SemiColon).Match(semiColon =>
                {
                    BreakStmt breakStmt = new BreakStmt(breakToken, semiColon);
                    return new Option<Result<Statement, ParserError>>(breakStmt);
                },
                error =>
                {
                    return new Option<Result<Statement, ParserError>>(error);
                });

            }
            else if (reader.Match(TokenType.Continue))
            {
                Token continueToken = reader.Previous();
                return reader.Consume(TokenType.SemiColon).Match(semiColon =>
                {
                    ContinueStmt breakStmt = new ContinueStmt(continueToken, semiColon);
                    return new Option<Result<Statement, ParserError>>(breakStmt);
                },
                error =>
                {
                    return new Option<Result<Statement, ParserError>>(error);
                });
            }

            return new Option<Result<Statement, ParserError>>();
        }

        private static Option<Result<IfStmt, List<ParserError>>> TryParseIf(ref TokenReader reader)
        {
            if (!reader.Match(TokenType.If))
                return new Option<Result<IfStmt, List<ParserError>>>();

            Token ifToken = reader.Previous();
            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return new Option<Result<IfStmt, List<ParserError>>>(new List<ParserError> { openParen.Error });

            List<ParserError> errors = new List<ParserError>();

            var expr = ExpressionParser.ParseExpression(ref reader);
            if(expr.IsError())
            {
                errors.Add(expr.Error);
                reader.SyncronizeTo(TokenType.CloseParen);
            }

            var closeParen = reader.Consume(TokenType.CloseParen);
            if (closeParen.IsError())
                errors.Add(closeParen.Error);

            var body = ParseStatement(ref reader);
            if (body.IsError())
                errors.AddRange(errors);

            Token? elseToken = null;
            Option<Statement> elseStatement = new Option<Statement>();
            if(reader.Match(TokenType.Else))
            {
                elseToken = reader.Previous();
                ParseStatement(ref reader).Match(
                    ok => elseStatement = ok, 
                    error => errors.AddRange(error));
            }

            if (errors.Any())
                return new Option<Result<IfStmt, List<ParserError>>>(errors);

            IfStmt ifStmt = new IfStmt(ifToken, openParen.Value, expr.Value, closeParen.Value, body.Value, elseToken, elseStatement);
            return new Option<Result<IfStmt, List<ParserError>>>(ifStmt);
        }

        private static Option<Result<ForStmt, List<ParserError>>> TryParseFor(TokenReader reader)
        {
            if (!reader.Match(TokenType.For))
                return new Option<Result<ForStmt, List<ParserError>>>();

            Token forToken = reader.Previous();

            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return new Option<Result<ForStmt, List<ParserError>>>(new List<ParserError> { openParen.Error });

            List<ParserError> errors = new List<ParserError>();

            var varDeclResult = TryParseVarDecl(ref reader);
            Option<VarDecl> init = new Option<VarDecl>();
            if(varDeclResult.HasValue())
            {
                varDeclResult.Value.Match(
                    ok => init = ok, 
                    error => 
                    { 
                        errors.Add(error);
                        reader.SyncronizeToAndAdvance(TokenType.SemiColon); // want to move past the semicolon
                    });
            }

            Option<Expression> condition = new Option<Expression>();
            if (reader.Current().Type != TokenType.SemiColon)
            {
                ExpressionParser.ParseExpression(ref reader).Match(
                    ok =>
                    {
                        condition = ok;
                    },
                    fail =>
                    {
                        errors.Add(fail); 
                        reader.SyncronizeTo(TokenType.SemiColon); // dont want to advance past semicolon
                    });
            }
            reader.Consume(TokenType.SemiColon);

            Option<Expression> iter = new Option<Expression>();
            if (reader.Current().Type != TokenType.CloseParen)
            {
                ExpressionParser.ParseExpression(ref reader).Match(
                    ok =>
                    {
                        iter = ok;
                    },
                    fail =>
                    {
                        errors.Add(fail);
                        reader.SyncronizeTo(TokenType.CloseParen); // dont want to advance past semicolon
                    });
            }
                
            var closeParen = reader.Consume(TokenType.CloseParen);
            if(closeParen.IsError())
            {
                errors.Add(closeParen.Error);
                return new Option<Result<ForStmt, List<ParserError>>>(errors);
            }

            var body = ParseStatement(ref reader);
            if (body.IsError())
                errors.AddRange(body.Error);

            if (errors.Any())
                return new Option<Result<ForStmt, List<ParserError>>>(errors);

            ForStmt forStmt = new ForStmt(forToken, openParen.Value, init.Value, condition, iter, closeParen.Value, body.Value);
            return new Option<Result<ForStmt, List<ParserError>>>(forStmt);
        }

        private static Option<Result<WhileStmt, List<ParserError>>> TryParseWhile(ref TokenReader reader)
        {
            if (!reader.Match(TokenType.While))
                return new Option<Result<WhileStmt, List<ParserError>>>();

            Token whileToken = reader.Previous();
            var openParen = reader.Consume(TokenType.OpenParen);
            if (openParen.IsError())
                return new Option<Result<WhileStmt, List<ParserError>>>(new List<ParserError> { openParen.Error });

            List<ParserError> errors = new List<ParserError>();

            var condition = ExpressionParser.ParseExpression(ref reader);
            if(condition.IsError())
            {
                errors.Add(condition.Error);
                reader.SyncronizeTo(TokenType.CloseParen); 
            }

            var closeParen = reader.Consume(TokenType.CloseParen);
            if (closeParen.IsError())
            {
                errors.Add(closeParen.Error);
                return new Option<Result<WhileStmt, List<ParserError>>>(errors);
            }

            var body = ParseStatement(ref reader);
            if(body.IsError())
            {
                errors.AddRange(body.Error);
                return new Option<Result<WhileStmt, List<ParserError>>>(errors);
            }

            if (errors.Any())
                return new Option<Result<WhileStmt, List<ParserError>>>(errors);

            WhileStmt whileStmt = new WhileStmt(whileToken, openParen.Value, condition.Value, closeParen.Value, body.Value);
            return new Option<Result<WhileStmt, List<ParserError>>>(whileStmt);
        }

        private static Option<Result<UnsafeBlock, List<ParserError>>> TryParseUnsafeBlock(TokenReader reader)
        {
            if(reader.Match(TokenType.Unsafe))
            {
                Token unsafeToken = reader.Previous();
                var openBrace = reader.Consume(TokenType.OpenBrace);
                if (openBrace.IsError())
                    return new Option<Result<UnsafeBlock, List<ParserError>>>(new List<ParserError> { openBrace.Error });

                List<Statement> statements = new List<Statement>();
                List<ParserError> errors = new List<ParserError>();

                while (!reader.IsAtEnd() && !reader.Match(TokenType.CloseBrace))
                {
                    ParseStatement(ref reader).Match(statement =>
                    {
                        statements.Add(statement);
                    },
                    error =>
                    {
                        errors.AddRange(error);
                        reader.SyncronizeTo(TokenType.For, TokenType.If, TokenType.CloseBrace, TokenType.OpenBrace);
                    });
                }

                if (errors.Any())
                    return new Option<Result<UnsafeBlock, List<ParserError>>>(errors);

                Token closeBrace = reader.Previous();

                UnsafeBlock unsafeBlock = new UnsafeBlock(unsafeToken, openBrace.Value, statements, closeBrace);
                return new Option<Result<UnsafeBlock, List<ParserError>>>(unsafeBlock);
            }

            return new Option<Result<UnsafeBlock, List<ParserError>>>();
        }

        private static Option<Result<ReturnStmt, ParserError>> TryParseReturn(ref TokenReader reader)
        {
            if (!reader.Match(TokenType.Return))
                return new Option<Result<ReturnStmt, ParserError>>();

            Token returnToken = reader.Previous();
            if(reader.Match(TokenType.SemiColon))
            {
                ReturnStmt returnStmt = new ReturnStmt(returnToken, null, reader.Previous());
                return new Option<Result<ReturnStmt, ParserError>>(returnStmt);
            }

            var expr = ExpressionParser.ParseExpression(ref reader);
            if (expr.IsError())
                return new Option<Result<ReturnStmt, ParserError>>(expr.Error);

            var semiColon = reader.Consume(TokenType.SemiColon);
            if (semiColon.IsError())
                return new Option<Result<ReturnStmt, ParserError>>(semiColon.Error);

            ReturnStmt statment = new ReturnStmt(returnToken, expr.Value, semiColon.Value);
            return new Option<Result<ReturnStmt, ParserError>>(statment);
        }

        private static Option<Result<VarDecl, ParserError>> TryParseVarDecl(ref TokenReader reader)
        {
            if (reader.Current().IsType(TokenType.Unsafe))
            {
                if (!IsVarDecl(reader, 1)) // offset by one
                    return new Option<Result<VarDecl, ParserError>>();
            }
            else
            {
                if (!IsVarDecl(reader))
                    return new Option<Result<VarDecl, ParserError>>();
            }

            Token? unsafeToken = reader.TryMatch(TokenType.Unsafe);
            var type = TypeNameParser.ParseTypeName(ref reader);
            if (type.IsError())
                return new Option<Result<VarDecl, ParserError>>(type.Error);

            Token? mutToken = reader.TryMatch(TokenType.Mut);
            List<Token> varNames = new List<Token>();
            varNames.Add(reader.Advance());

            while(reader.Match(TokenType.Comma))
            {
                var id = reader.Consume(TokenType.Identifier);
                if (id.IsError())
                    return new Option<Result<VarDecl, ParserError>>(id.Error);

                varNames.Add(id.Value);
            }

            if(reader.Match(TokenType.SemiColon))
            {
                VarDecl varDecl = new VarDecl(unsafeToken, type.Value, mutToken, varNames, null, null, reader.Previous());
                return new Option<Result<VarDecl, ParserError>>(varDecl);
            }
            else
            {
                var equel = reader.Consume(TokenType.Equal);
                if (equel.IsError())
                    return new Option<Result<VarDecl, ParserError>>(equel.Error);

                var expr = ExpressionParser.ParseExpression(ref reader);
                if (expr.IsError())
                    return new Option<Result<VarDecl, ParserError>>(expr.Error);

                var semiColon = reader.Consume(TokenType.SemiColon);
                if (semiColon.IsError())
                    return new Option<Result<VarDecl, ParserError>>(semiColon.Error);

                VarDecl varDecl = new VarDecl(unsafeToken, type.Value, mutToken, varNames, equel.Value, expr.Value, semiColon.Value);
                return new Option<Result<VarDecl, ParserError>>(varDecl);
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

        private static bool IsClassDecl(TokenReader reader)
        {
            return reader.CheckSequence(TokenType.Class) || 
                   reader.CheckSequence(TokenType.Unsafe, TokenType.Class);
        }

        private static bool IsMemberDecl(TokenReader reader)
        {
            int offset = reader.CurrentIs(TokenType.Public, TokenType.Private) ? 1 : 0;
            return IsFunctionDecl(reader, offset)       || 
                   IsVarDecl(reader, offset)            || 
                   IsConstructorDecl(reader, offset)    || 
                   IsDestructorDecl(reader, offset);
        }

        private static bool IsFunctionDecl(TokenReader reader, int offset)
        {
            return reader.CheckSequence(offset, TokenType.Func) || 
                   reader.CheckSequence(offset, TokenType.Unsafe, TokenType.Func);
        }

        private static bool IsConstructorDecl(TokenReader reader, int offset)
        {
            return reader.CheckSequence(offset, TokenType.Identifier, TokenType.OpenParen) ||
                   reader.CheckSequence(offset, TokenType.Unsafe, TokenType.Identifier, TokenType.OpenParen);
        }

        private static bool IsDestructorDecl(TokenReader reader, int offset)
        {
            return reader.CheckSequence(offset, TokenType.Tilda, TokenType.Identifier) || 
                   reader.CheckSequence(offset, TokenType.Unsafe, TokenType.Tilda, TokenType.Identifier);
        }

        private static Option<Result<BlockStmt, List<ParserError>>> TryParseBlock(TokenReader reader)
        {
            if(reader.Match(TokenType.OpenBrace))
            {
                Token openBrace = reader.Previous();

                List<Statement> statements = new List<Statement>();
                List<ParserError> errors = new List<ParserError>();

                while (!reader.IsAtEnd() && !reader.Match(TokenType.CloseBrace))
                {
                    ParseStatement(ref reader).Match(statement =>
                    {
                        statements.Add(statement);
                    },
                    error =>
                    {
                        errors.AddRange(error);
                        reader.SyncronizeTo(TokenType.For, TokenType.If, TokenType.CloseBrace, TokenType.OpenBrace);
                    });
                }

                if (errors.Any())
                    return new Option<Result<BlockStmt, List<ParserError>>>(errors);

                Token closeBrace = reader.Previous();

                BlockStmt block = new BlockStmt(openBrace, statements, closeBrace);
                return new Option<Result<BlockStmt, List<ParserError>>>(block);
            }

            return new Option<Result<BlockStmt, List<ParserError>>>();
        }

        private static Result<Statement, ParserError> ParseExpressionStatement(ref TokenReader reader)
        {
            var expr = ExpressionParser.ParseExpression(ref reader);
            if (expr.IsError())
                return expr.Error;

            var semiColon = reader.Consume(TokenType.SemiColon);
            if (semiColon.IsError())
                return semiColon.Error;

            return new ExprStmt(expr.Value, semiColon.Value);
        }
    }
}
