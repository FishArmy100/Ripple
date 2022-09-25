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
    using CompilerResult = Result<TranspilerResult, List<CompilerError>>;

    static class Compiler
    {
        public static CompilerResult Compile(string rippleSource, string outputFileName)
        {
            var lexerResult = Lexer.Scan(rippleSource);

            if(lexerResult is Result<List<Token>, List<LexerError>>.Fail f)
            {
                List<CompilerError> errors = f.Error.ConvertAll(e => new CompilerError(e));
                return new CompilerResult.Fail(errors);
            }

            List<Token> tokens = (lexerResult as Result<List<Token>, List<LexerError>>.Ok).Data;

            var parserResult = Parser.Parse(tokens);

            if(parserResult is Result<FileStmt, List<ParserError>>.Fail parserFailer)
            {
                List<CompilerError> errors = parserFailer.Error.ConvertAll(x => new CompilerError(x));
                return new CompilerResult.Fail(errors);
            }

            FileStmt file = (parserResult as Result<FileStmt, List<ParserError>>.Ok).Data;

            List<ValidationError> validationErrors = Validator.ValidateAst(file);

            if(validationErrors.Count > 0)
            {
                List<CompilerError> errors = validationErrors.ConvertAll(x => new CompilerError(x));
                return new CompilerResult.Fail(errors);
            }

            TranspilerResult transpilerResult = Transpiler.Transpile(file, outputFileName);
            return new CompilerResult.Ok(transpilerResult);
        }
    }
}
