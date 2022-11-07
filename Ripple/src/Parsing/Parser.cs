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
        public static Result<ProgramStmt, List<ParserError>> Parse(List<Token> tokens)
        {
            TokenReader reader = new TokenReader(tokens);
            List<ParserError> errors = new List<ParserError>();
            List<FileStmt> files = new List<FileStmt>();

            while (!reader.IsAtEnd())
            {
                try
                {
                    FileStmt file = ParseFile(ref reader, ref errors);
                    files.Add(file);
                }
                catch (ParserExeption e)
                {
                    errors.Add(new ParserError(e.Message, e.Tok));
                    reader.SyncronizeTo(TokenType.EOF);

                    if (!reader.IsAtEnd())
                        reader.Advance(); // go passed EOF token
                }
                catch(ReaderAtEndExeption)
                {
                    errors.Add(new ParserError("Reader passed the end of the token list.", new Token()));
                    break;
                }
            }

            if (errors.Count > 0)
                return new Result<ProgramStmt, List<ParserError>>.Fail(errors);

            return new Result<ProgramStmt, List<ParserError>>.Ok(new ProgramStmt(files));
        }

        private static FileStmt ParseFile(ref TokenReader reader, ref List<ParserError> errors)
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
                    errors.Add(new ParserError(e.Message, e.Tok));
                    reader.SyncronizeTo(reader =>
                    {
                        return reader.Current().IsType(TokenType.Func) || 
                               IsVarDecl(reader) ||
                               reader.Current().IsType(TokenType.EOF);
                    });
                }
            }

            Token eof = reader.Consume(TokenType.EOF, "Expected end of file.");
            return new FileStmt(declarations, eof.Text, eof);
        }

        private static Statement ParseDeclaration(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (TryParseFunctionDecl(ref reader, ref errors, out FuncDecl func))
                return func;
            else if (TryParseVarDecl(ref reader, out Statement statement))
                return statement;
            else if (TryParseExernalFunctionDecl(ref reader, out ExternalFuncDecl funcDecl))
                return funcDecl;

            throw new ParserExeption(reader.Current(), "Expected a declaration.");
        }

        private static bool TryParseFunctionDecl(ref TokenReader reader, ref List<ParserError> errors, out FuncDecl funcDecl)
        {
            funcDecl = null;
            if (!reader.Match(TokenType.Func))
                return false;

            Token func = reader.Previous();
            Token name = reader.Consume(TokenType.Identifier, "Expected function name.");
            Parameters parameters = ParseParameters(ref reader);
            Token arrow = reader.Consume(TokenType.RightThinArrow, "Expected '->'.");
            TypeName returnType = ParseTypeName(ref reader);
            if (!TryParseBlock(ref reader, ref errors, out BlockStmt body))
                throw new ParserExeption(reader.Current(), "Expected a function body.");

            funcDecl = new FuncDecl(func, name, parameters, arrow, returnType, body);
            return true;
        }

        private static bool TryParseExernalFunctionDecl(ref TokenReader reader, out ExternalFuncDecl externalFuncDecl)
        {
            externalFuncDecl = null;
            if (!reader.Match(TokenType.Extern))
                return false;

            Token externToken = reader.Previous();
            Token stringLit = reader.Consume(TokenType.StringLiteral, "Expected a language name.");
            Token funkToken = reader.Consume(TokenType.Func, "Expected 'func'.");
            Token funcName = reader.Consume(TokenType.Identifier, "Expected a function name.");
            Parameters parameters = ParseParameters(ref reader);
            Token arrow = reader.Consume(TokenType.RightThinArrow, "Expected '->'.");
            TypeName returnType = TypeNameHelper.ParseTypeName(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon, "Expected a ';'.");

            externalFuncDecl = new ExternalFuncDecl(externToken, stringLit, funkToken, funcName, parameters, arrow, returnType, semiColon);
            return true;
        }

        private static Parameters ParseParameters(ref TokenReader reader)
        {
            Token openParen = reader.Consume(TokenType.OpenParen, "Expected '('.");
            List<(TypeName, Token)> parameters = new List<(TypeName, Token)>();
            while(!reader.IsAtEnd() && !reader.Current().IsType(TokenType.CloseParen))
            {
                reader.Match(TokenType.Comma);
                TypeName typeName = ParseTypeName(ref reader);
                Token paramName = reader.Consume(TokenType.Identifier, "Expected parameter name.");
                parameters.Add((typeName, paramName));
            }

            Token closeParen = reader.Consume(TokenType.CloseParen, "Expected ')'");

            return new Parameters(openParen, parameters, closeParen);
        }

        private static Statement ParseStatement(ref TokenReader reader, ref List<ParserError> errors)
        {
            if (TryParseIf(ref reader, ref errors, out Statement statement))
                return statement;
            else if (TryParseBlock(ref reader, ref errors, out BlockStmt block))
                return block;
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

            Token openParen = reader.Consume(TokenType.OpenParen, "Expected '('.");

            if (!TryParseVarDecl(ref reader, out Statement init))
                init = null;

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
            if(reader.Match(TokenType.SemiColon))
            {
                statement = new ReturnStmt(returnToken, null, reader.Previous());
                return true;
            }

            Expression expr = ParseExpression(ref reader);

            Token semiColon = reader.Consume(TokenType.SemiColon, "Expected ';' after return statement");

            statement = new ReturnStmt(returnToken, expr, semiColon);
            return true;
        }

        public static bool TryParseVarDecl(ref TokenReader reader, out Statement statement)
        {
            statement = null;
            if (!IsVarDecl(reader))
                return false;

            TypeName type = ParseTypeName(ref reader);
            List<Token> varNames = new List<Token>();
            varNames.Add(reader.Advance());

            while(reader.Match(TokenType.Comma))
            {
                varNames.Add(reader.Consume(TokenType.Identifier, "Expected variable name."));
            }

            Token equel = reader.Consume(TokenType.Equal, "Expected '='.");

            Expression expr = ParseExpression(ref reader);
            Token semiColon = reader.Consume(TokenType.SemiColon, "Expected ';' after variable declaration.");

            statement = new VarDecl(type, varNames, equel, expr, semiColon);
            return true;
        }

        private static bool IsVarDecl(TokenReader reader)
        {
            if(IsTypeName(ref reader, out int offset))
            {
                return reader.Peek(offset) is Token t && 
                       t.Type.IsIdentifier();
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
            Expression obj = ParseCasting(ref reader);
            if(obj is AST.Index || obj is Unary u && u.Op.Type == TokenType.Star) // is an index, or dereference
            {
                Token equles = reader.Consume(TokenType.Equal, "Expected a '='.");
                Expression value = ParseAssignment(ref reader);
                return new Binary(obj, equles, value);
            }

            return obj;
        }

        private static Expression ParseCasting(ref TokenReader reader)
        {
            Expression expression = ParseLogicalOr(reader);
            while(reader.Match(TokenType.As))
            {
                Token asToken = reader.Previous();
                TypeName typeName = TypeNameHelper.ParseTypeName(ref reader);
                expression = new Cast(expression, asToken, typeName);
            }

            return expression;
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
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private static Expression ParseUnaryExpr(TokenReader reader)
        {
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
                        reader.Consume(TokenType.Comma, "Expected a ','.");
                }
                Token closeParen = reader.Consume(TokenType.CloseParen, "Expected a ')'.");
                Expression call = new Call(callee, openParen, arguments, closeParen);
                return ParseCallOrIndexArgs(reader, call);
            }
            else if(reader.Match(TokenType.OpenBracket))
            {
                Token openBracket = reader.Previous();
                Expression expression = ParseExpression(ref reader);
                Token closeBracket = reader.Consume(TokenType.CloseBracket, "Expected a ']'.");
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
            if (reader.Match(TokenType.IntagerLiteral, TokenType.FloatLiteral, TokenType.True, TokenType.False, TokenType.StringLiteral, TokenType.CharactorLiteral))
                return new Literal(reader.Previous());
            else if (reader.Match(TokenType.Identifier))
                return new Identifier(reader.Previous());
            else if (ParseGrouping(reader, out Expression expr))
                return expr;
            else if (ParseInitializerList(reader, out expr))
                return expr;
            else
            {
                throw new ParserExeption(reader.Current(), "Expected expression");
            }
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
                    reader.Consume(TokenType.Comma, "Expected a ','.");
            }
            Token closeBrace = reader.Consume(TokenType.CloseBrace, "Expected a '}'.");

            initalizerList =  new InitializerList(openBrace, expressions, closeBrace);
            return true;
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

        private static TypeName ParseTypeName(ref TokenReader reader)
        {
            return TypeNameHelper.ParseTypeName(ref reader);
        }

        private static bool IsTypeName(ref TokenReader reader, out int length)
        {
            return TypeNameHelper.IsTypeName(ref reader, out length);
        }
    }
}
