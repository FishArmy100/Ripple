using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    struct ASTType
    {
        public readonly string Name;

        public ASTType(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is ASTType type &&
                   Name == type.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
