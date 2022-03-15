using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    interface IASTVisitor<T> : IExpressionVisitor<T>, IStatementVisitor<T>
    {
    }

    interface IASTVisitor : IExpressionVisitor, IStatementVisitor
    {
    }
}
