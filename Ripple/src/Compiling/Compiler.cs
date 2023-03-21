using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Validation;
using Ripple.Utils;
using Ripple.AST;
using Ripple.Validation.Info.Statements;
using Ripple.Transpiling.ASTConversion;

namespace Ripple.Compiling
{
    static class Compiler
    {
        public static Result<List<Token>, List<CompilerError>> RunLexer(List<SourceFile> sourceFiles)
        {
            var results = sourceFiles
                .ConvertAll(f => Lexer.Scan(f.Source, f.Path))
                .ConvertAll(lr => lr.ConvertToCompilerResult(e => new CompilerError(e)));

            List<Token> tokens = new List<Token>();
            List<CompilerError> compilerErrors = new List<CompilerError>();

            foreach(var result in results)
            {
                result.Match(
                    ok =>
                    {
                        tokens.AddRange(ok);
                    },
                    fail =>
                    {
                        compilerErrors.AddRange(fail);
                    });
            }

            if (compilerErrors.Count > 0)
                return compilerErrors;
            else
                return tokens;
        }

        public static Result<ProgramStmt, List<CompilerError>> RunParser(List<SourceFile> sourceFiles)
        {
            return RunLexer(sourceFiles).Match(
                    ok => Parser.Parse(ok).ConvertToCompilerResult(e => new CompilerError(e)), 
                    fail => new Result<ProgramStmt, List<CompilerError>>(fail));
        }

        public static Result<TypedProgramStmt, List<CompilerError>> RunValidator(List<SourceFile> sourceFiles)
        {
            return RunParser(sourceFiles).Match(
                    ok => Validator.ValidateAst(ok).ConvertToCompilerResult(e => new CompilerError(e)),
                    fail => new Result<TypedProgramStmt, List<CompilerError>>(fail));
        }

        public static Result<string, List<CompilerError>> RunTranspiler(List<SourceFile> sourceFiles)
        {
            return RunValidator(sourceFiles).Match(
                    ok => ASTConverter.ConvertAST(ok),
                    fail => new Result<string, List<CompilerError>>(fail));
        }

        private static Result<TSuccess, List<CompilerError>> ConvertToCompilerResult<TSuccess, TError>(this Result<TSuccess, List<TError>> self, Converter<TError, CompilerError> converter)
        {
            return self.Match(
                ok   => new Result<TSuccess, List<CompilerError>>(ok),
                fail => fail.ConvertAll(converter));
        }
    }
}
