using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Utils;
using Ripple.Core;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class AssignmentError : ValidationError
    {
        public readonly TypeInfo AssignedType;
        public readonly TypeInfo ExpressionType;

        public AssignmentError(SourceLocation location, TypeInfo assignedType, TypeInfo expressionType) : base(location)
        {
            AssignedType = assignedType;
            ExpressionType = expressionType;
        }

        public override string GetMessage() => 
            $"Cannot assign type {ExpressionType.ToPrettyString()} " +
            $"to type {AssignedType.ToPrettyString()}";
    }
}
