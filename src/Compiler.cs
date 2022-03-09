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
            errors.AddRange(scannerResult.Errors.ConvertAll((sError => new CompilerError(sError))));

            var parserResult = Parser.Parse(scannerResult.Result);
            errors.AddRange(parserResult.Errors.ConvertAll((pError => new CompilerError(pError))));

            CompilerResult result = new CompilerResult(scannerResult.Result, parserResult.Result);
            return new OperationResult<CompilerResult, CompilerError>(result, errors);
        }
    }
}
