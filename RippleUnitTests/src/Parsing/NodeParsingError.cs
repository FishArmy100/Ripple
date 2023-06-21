using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleUnitTests.Parsing
{
    class NodeParsingError
    {
        public readonly string Message;

        public NodeParsingError(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"Node parsing error: {Message}";
        }
    }
}
