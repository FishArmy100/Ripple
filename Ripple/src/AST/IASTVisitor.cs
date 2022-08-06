using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    interface IASTVisitor<T> : IExpressionVisitor<T>, IStatementVisitor<T>, IRippleTypeVisitor<T>
    {
    }

    interface IASTVisitor : IExpressionVisitor, IStatementVisitor, IRippleTypeVisitor
    {
    }
}
