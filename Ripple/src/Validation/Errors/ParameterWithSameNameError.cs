using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class ParameterWithSameNameError : ValidationError
    {
        public readonly string ParameterName;

        public ParameterWithSameNameError(SourceLocation location, string parameterName) : base(location)
        {
            ParameterName = parameterName;
        }

        public override string GetMessage()
        {
            return $"A parameter with name '{ParameterName}' already exists.";
        }
    }
}
