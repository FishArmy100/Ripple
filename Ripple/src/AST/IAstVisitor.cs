using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    interface IAstVisitor : IExpressionVisitor, IStatementVisitor, ITypeNameVisitor
    {
    }
}
