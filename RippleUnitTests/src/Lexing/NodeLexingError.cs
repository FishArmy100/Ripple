using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleUnitTests.Lexing
{
    class NodeLexingError
    {
        public readonly string Message;

        public NodeLexingError(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"Node parsing error: {Message}";
        }
    }
}
