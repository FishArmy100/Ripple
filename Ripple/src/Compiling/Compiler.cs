using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Validation;
using Ripple.Transpiling;
using Ripple.Utils;
using Ripple.AST;

namespace Ripple.Compiling
{
    static class Compiler
    {
        public static CompilerResult<TranspilerResult> Compile(string source, string fileName)
        {
            return Lexer.Scan(source).ConvertToCompilerResult(e => new CompilerError(e))
                .Match(tokens => Parser.Parse(tokens).ConvertToCompilerResult(e => new CompilerError(e)))
                .Match(file => Validator.ValidateAst(file).ConvertToCompilerResult(e => new CompilerError(e)))
                .Match(file =>
                {
                    var result = Transpiler.Transpile(file, fileName);
                    return new CompilerResult<TranspilerResult>.Ok(result);
                });
        }

        public static CompilerResult<List<Token>> RunLexer(string source)
        {
            return Lexer.Scan(source)
                .ConvertToCompilerResult(e => new CompilerError(e));
        }

        public static CompilerResult<FileStmt> RunParser(string source)
        {
            return RunLexer(source)
                .Match(ok => Parser.Parse(ok)
                .ConvertToCompilerResult(e => new CompilerError(e)));
        }

        public static CompilerResult<FileStmt> RunValidator(string source)
        {
            return RunParser(source)
                .Match(file => Validator.ValidateAst(file)
                .ConvertToCompilerResult(e => new CompilerError(e)));
        }

        private static CompilerResult<TSuccess> ConvertToCompilerResult<TSuccess, TError>(this Result<TSuccess, List<TError>> self, Converter<TError, CompilerError> converter)
        {
            return self switch
            {
                Result<TSuccess, List<TError>>.Ok ok => new CompilerResult<TSuccess>.Ok(ok.Data),
                Result<TSuccess, List<TError>>.Fail fail => new CompilerResult<TSuccess>.Fail(fail.Error.ConvertAll(converter)),
                _ => throw new ArgumentException("Result has a different result than ok, or fail")
            };
        }
    }
}
