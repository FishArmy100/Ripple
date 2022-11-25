using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    struct PrimaryTypeInfo
    {
        public readonly Token Name;

        public PrimaryTypeInfo(Token name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is PrimaryTypeInfo info &&
                   EqualityComparer<Token>.Default.Equals(Name, info.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
