using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleUnitTests.Lexing
{
    class TestNode
    {
        public readonly string Code;
        public readonly IReadOnlyList<ArgumentToken> Arguments;

        public TestNode(string code, IReadOnlyList<ArgumentToken> arguments)
        {
            Code = code;
            Arguments = arguments;
        }
    }
}
