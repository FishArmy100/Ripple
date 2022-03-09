using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    interface IASTVisitor<T> : Expression.IExpressionVisitor<T>, Statement.IStatementVisitor<T>
    {
    }

    interface IASTVisitor : Expression.IExpressionVisitor, Statement.IStatementVisitor
    {
    }
}
