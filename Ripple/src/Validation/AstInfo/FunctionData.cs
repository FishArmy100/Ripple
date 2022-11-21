using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class FunctionData
    {
        public readonly Token Name;
        public readonly List<(Token, Token)> Parameters;
        public readonly Token ReturnType;

        public FunctionData(Token name, List<(Token, Token)> parameters, Token returnType)
        {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
