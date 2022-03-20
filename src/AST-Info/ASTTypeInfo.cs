using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Info
{
    struct ASTTypeInfo
    {
        public readonly string Name;

        public ASTTypeInfo(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is ASTTypeInfo type &&
                   Name == type.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
