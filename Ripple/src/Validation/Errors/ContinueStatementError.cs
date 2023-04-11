using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Errors
{
    class ContinueStatementError : ValidationError
    {
        public ContinueStatementError(SourceLocation location) : base(location)
        {
        }

        public override string GetMessage() => 
            "A continue statement must be in a if, for, or while statement.";
    }
}
