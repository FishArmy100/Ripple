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
using Ripple.AST.Info;

namespace Ripple.Compiling
{
    static class Compiler
    {
        public static CompilerResult<List<Token>> RunLexer(List<SourceFile> sourceFiles)
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
                return new CompilerResult<List<Token>>.Fail(compilerErrors);
            else
                return new CompilerResult<List<Token>>.Ok(tokens);
        }

        public static CompilerResult<ProgramStmt> RunParser(List<SourceFile> sourceFiles)
        {
            return RunLexer(sourceFiles)
                .Match(ok => Parser.Parse(ok)
                .ConvertToCompilerResult(e => new CompilerError(e)));
        }

        public static CompilerResult<ASTInfo> RunValidator(List<SourceFile> sourceFiles)
        {
            return RunParser(sourceFiles)
                .Match(ok => Validator.ValidateAst(ok)
                .ConvertToCompilerResult(e => new CompilerError(e)));
        }

        private static CompilerResult<TSuccess> ConvertToCompilerResult<TSuccess, TError>(this Result<TSuccess, List<TError>> self, Converter<TError, CompilerError> converter)
        {
            return self.Match(
                ok   => (CompilerResult<TSuccess>)new CompilerResult<TSuccess>.Ok(ok),
                fail => (CompilerResult<TSuccess>)new CompilerResult<TSuccess>.Fail(fail.ConvertAll(converter)));
        }
    }
}
