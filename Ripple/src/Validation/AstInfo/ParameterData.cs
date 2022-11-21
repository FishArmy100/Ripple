using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Validation.AstInfo
{
    class ParameterData
    {
        readonly Token Name;
        readonly TypeName Type;

        public ParameterData(Token name, TypeName type)
        {
            Name = name;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is ParameterData data &&
                   EqualityComparer<Token>.Default.Equals(Name, data.Name) &&
                   EqualityComparer<TypeName>.Default.Equals(Type, data.Type);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }
    }
}
