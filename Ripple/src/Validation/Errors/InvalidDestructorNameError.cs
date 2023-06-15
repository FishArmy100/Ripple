using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class InvalidDestructorNameError : ValidationError
    {
        public readonly string ClassName;
        public readonly string DestructorName;

        public InvalidDestructorNameError(SourceLocation location, string className, string destructorName) : base(location)
        {
            DestructorName = destructorName;
            ClassName = className;
        }

        public override string GetMessage()
        {
            return "Destructor must have the same name as its declaring class.";
        }
    }
}
