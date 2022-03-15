using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class FunctionParameter
    {
        public readonly Token Name;
        public readonly Token Type;

        public FunctionParameter(Token name, Token type)
        {
            Name = name;
            Type = type;
        }
    }
}
