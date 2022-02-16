using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    class ScanResult
    {
        public readonly List<Token> Tokens;
        public readonly List<ScannerError> ScannerErrors;
        public bool HadError => ScannerErrors.Count > 0;

        public ScanResult(List<Token> tokens, List<ScannerError> errors)
        {
            Tokens = tokens;
            ScannerErrors = errors;
        }
    }
}
