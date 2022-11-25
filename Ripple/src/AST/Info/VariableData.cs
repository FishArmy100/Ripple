using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;

namespace Ripple.AST.Info
{
    class VariableData
    {
        public readonly Token Name;
        public readonly TypeInfo Type;

        public VariableData(Token name, TypeInfo type)
        {
            Name = name;
            Type = type;
        }

        public string GetName() => Name.Text;
    }
}
