using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    abstract class Statement
    {
        public abstract void Accept(IStatementVisitor visitor);
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}
