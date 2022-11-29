using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Utils
{
    interface IAstVisitor : IExpressionVisitor, IStatementVisitor, ITypeNameVisitor
    {
    }

    interface IAstVisitor<T> : IExpressionVisitor<T>, IStatementVisitor<T> 
    {
    }
}
