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
        public readonly Token NameToken;
        public readonly TypeInfo Type;

        public string Name => NameToken.Text;

        public ParameterInfo(Token name, TypeInfo type)
        {
            NameToken = name;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is ParameterInfo info &&
                   EqualityComparer<Token>.Default.Equals(NameToken, info.NameToken) &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NameToken, Type);
        }
    }
}
