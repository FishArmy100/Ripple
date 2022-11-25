using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class ParameterInfo
    {
        public readonly Token Name;
        public readonly TypeInfo Type;

        public ParameterInfo(Token name, TypeInfo type)
        {
            Name = name;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is ParameterInfo info &&
                   EqualityComparer<Token>.Default.Equals(Name, info.Name) &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }
    }
}
