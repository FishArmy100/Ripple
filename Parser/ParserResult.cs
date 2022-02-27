using System;
using System.Collections.Generic;
using System.Text;

namespace Ripple
{
    class ParserResult
    {
        public readonly Expression ParsedExpression;
        public readonly List<ParserError> Errors;
        public bool HasError => Errors.Count > 0;

        public ParserResult(Expression parsedExpression, List<ParserError> errors)
        {
            ParsedExpression = parsedExpression;
            Errors = errors;
        }
    }
}
