using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class ArrayInitalizerListTypeArrayError : ValidationError
    {
        public readonly TypeInfo Expected;
        public readonly TypeInfo Found;

        public ArrayInitalizerListTypeArrayError(SourceLocation location, TypeInfo expected, TypeInfo found) : base(location)
        {
            Expected = expected;
            Found = found;
        }

        public override string GetMessage()
        {
            string message = "Expected an array of: " +
                            Expected.ToPrettyString() +
                            ", but found an expression for: " +
                            Found.ToPrettyString() + ".";

            return message;
        }
    }
}
