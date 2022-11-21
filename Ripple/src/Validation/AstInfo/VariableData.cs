using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;

namespace Ripple.Validation.AstInfo
{
    class VariableData
    {
        public readonly Token Name;
        public readonly TypeName Type;

        public VariableData(Token name, TypeName type)
        {
            Name = name;
            Type = type;
        }

        public string GetName() => Name.Text;
    }
}
