using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Errors
{
    class ExternFunctionLinkerError : ValidationError
    {
        public readonly string ExternFunctionName;
        public readonly TypeInfo FunctionType;
        public ExternFunctionLinkerError(SourceLocation location, string externFunctionName, TypeInfo functionType) : base(location)
        {
            ExternFunctionName = externFunctionName;
            FunctionType = functionType;
        }

        public override string GetMessage() => 
            $"No external function found with name '{ExternFunctionName}' " +
            $"of type {FunctionType.ToPrettyString()}";
    }
}
