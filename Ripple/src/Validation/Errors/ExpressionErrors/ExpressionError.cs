using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Errors.ExpressionErrors
{
    class ExpressionError : ValidationError
    {
        public readonly ExpressionType Type;
        public readonly List<TypeInfo> Arguments;

        protected ExpressionError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage()
        {
            throw new NotImplementedException();
        }

        public enum ExpressionType
        {
            Unary,
            Binary,
            Index,
            Call,
            Cast
        }
    }
}
