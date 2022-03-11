using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;

namespace Ripple
{
    static class Compiler
    {
        public static OperationResult<CompilerResult, CompilerError> CompileSource(string src)
        {
            List<CompilerError> errors = new List<CompilerError>();

            var scannerResult = Scanner.GetTokens(src);
            errors.AddRange(scannerResult.Errors.ConvertAll((sError => ToCompilerError(sError))));

            var parserResult = Parser.Parse(scannerResult.Result);
            errors.AddRange(parserResult.Errors.ConvertAll((pError => ToCompilerError(pError))));

            CompilerResult result = new CompilerResult(scannerResult.Result, parserResult.Result);
            return new OperationResult<CompilerResult, CompilerError>(result, errors);
        }

        private static CompilerError ToCompilerError(ScannerError error)
        {
            return new CompilerError("None", error.Line, error.Column, error.Message);
        }

        private static CompilerError ToCompilerError(ParserError error)
        {
            return new CompilerError("None", error.Token.Line, error.Token.Column, error.Message);
        }
    }
}
