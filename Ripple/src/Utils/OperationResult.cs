using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    class OperationResult<TResult, TError>
    {
        public readonly TResult Result;
        public readonly List<TError> Errors;

        public OperationResult(TResult result, List<TError> errors)
        {
            Result = result;
            Errors = errors;
        }

        public bool HasError => Errors.Count > 0;
    }
}
