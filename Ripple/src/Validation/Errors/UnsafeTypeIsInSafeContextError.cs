using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Core;
using Ripple.AST.Utils;

namespace Ripple.Validation.Errors
{
    class UnsafeTypeIsInSafeContextError : ValidationError
    {
        public readonly TypeName TypeName;

        public UnsafeTypeIsInSafeContextError(SourceLocation location, TypeName typeName) : base(location)
        {
            TypeName = typeName;
        }

        public override string GetMessage() => $"Unsafe type '{TypeNamePrinter.PrintType(TypeName)}' in a safe context.";
    }
}
