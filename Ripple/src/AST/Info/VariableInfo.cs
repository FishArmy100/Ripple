using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.AST;

namespace Ripple.AST.Info
{
    class VariableInfo
    {
        public readonly Token NameToken;
        public readonly TypeInfo Type;
        public readonly bool IsUnsafe;
        public readonly int Lifetime;

        public VariableInfo(Token nameToken, TypeInfo type, bool isUnsafe, int lifetime)
        {
            NameToken = nameToken;
            Type = type;
            IsUnsafe = isUnsafe;
            Lifetime = lifetime;
        }

        public string Name => NameToken.Text;

        public override bool Equals(object obj)
        {
            return obj is VariableInfo info &&
                   EqualityComparer<Token>.Default.Equals(NameToken, info.NameToken) &&
                   EqualityComparer<TypeInfo>.Default.Equals(Type, info.Type) &&
                   IsUnsafe == info.IsUnsafe &&
                   Lifetime == info.Lifetime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NameToken, Type, IsUnsafe, Lifetime);
        }
    }
}
