using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class FunctionParameter
    {
        public readonly Token Name;
        public readonly RippleType Type;

        public FunctionParameter(Token name, RippleType type)
        {
            Name = name;
            Type = type;
        }
    }
}
