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
using Ripple.Transpiling;

namespace Ripple.Compiling
{
    static class Compiler
    {
        public const string INTERMEDIATE_FOLDER_NAME = "intermediates";
        public const string BIN_FOLDER_NAME = "bin";

        public static Result<List<Token>, List<CompilerError>> RunLexer(SourceData sourceFiles)
        {
            var results = sourceFiles.Files
                .ConvertAll(f => Lexer.Scan(f.Read(), f.RelativePath))
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

        public static Result<ProgramStmt, List<CompilerError>> RunParser(SourceData sourceFiles)
        {
            return RunLexer(sourceFiles).Match(
                    ok => Parser.Parse(ok, sourceFiles.StartPath).ConvertToCompilerResult(e => new CompilerError(e)), 
                    fail => new Result<ProgramStmt, List<CompilerError>>(fail));
        }

        public static Result<TypedProgramStmt, List<CompilerError>> RunValidator(SourceData sourceFiles)
        {
            return RunParser(sourceFiles).Match(
                    ok => Validator.ValidateAst(ok).ConvertToCompilerResult(e => new CompilerError(e)),
                    fail => new Result<TypedProgramStmt, List<CompilerError>>(fail));
        }

        public static Result<List<CFileInfo>, List<CompilerError>> RunTranspiler(SourceData sourceFiles)
        {
            return RunValidator(sourceFiles).Match(
                    ok => Transpiler.Transpile(ok),
                    fail => new Result<List<CFileInfo>, List<CompilerError>>(fail));
        }

        public static Result<string, List<CompilerError>> RunClangCompiler(SourceData sourceFiles)
        {
            return new Result<string, List<CompilerError>>(new List<CompilerError>());
        }

        private static Result<TSuccess, List<CompilerError>> ConvertToCompilerResult<TSuccess, TError>(this Result<TSuccess, List<TError>> self, Converter<TError, CompilerError> converter)
        {
            return self.Match(
                ok   => new Result<TSuccess, List<CompilerError>>(ok),
                fail => fail.ConvertAll(converter));
        }
    }
}
