using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class VariableData
    {
        public readonly Token Name;
        public readonly Token Type;

        public VariableData(Token name, Token type)
        {
            Name = name;
            Type = type;
        }

        public string GetName() => Name.Text;
    }
}
