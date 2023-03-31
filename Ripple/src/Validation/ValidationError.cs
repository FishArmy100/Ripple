using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    public struct ValidationError
    {
        public readonly string Message;
        public readonly Token ErrorToken;

        public ValidationError(string message, Token errorToken)
        {
            Message = message;
            ErrorToken = errorToken;
        }

        public override string ToString()
        {
            return "Validation Error: " + Message;
        }
    }
}
