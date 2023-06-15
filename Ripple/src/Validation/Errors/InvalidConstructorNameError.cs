using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class InvalidConstructorNameError : ValidationError
    {
        public readonly string ClassName;
        public readonly string ConstructorName;

        public InvalidConstructorNameError(SourceLocation location, string className, string constructorName) : base(location)
        {
            ClassName = className;
            ConstructorName = constructorName;
        }

        public override string GetMessage()
        {
            return $"Constructor must have the same name as its deriving class.";
        }
    }
}
