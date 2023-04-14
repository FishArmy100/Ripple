using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Core;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Errors
{
    class InvalidReturnTypeError : ValidationError
    {
        public readonly TypeInfo ExpressionType;
        public readonly TypeInfo FunctionReturnType;

        public InvalidReturnTypeError(SourceLocation location, TypeInfo expressionType, TypeInfo functionReturnType) : base(location)
        {
            ExpressionType = expressionType;
            FunctionReturnType = functionReturnType;
        }

        public override string GetMessage() => 
            $"Cannot return value of type '{ExpressionType.ToPrettyString()}' " +
            $"from a function that returns '{FunctionReturnType.ToPrettyString()}'.";
    }
}
