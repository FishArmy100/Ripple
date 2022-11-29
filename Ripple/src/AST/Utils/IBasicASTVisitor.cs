
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Utils
{
    interface IBasicASTVisitor : IStatementVisitor, IExpressionVisitor
    {
    }

    interface IBasicASTVisitor<T> : IStatementVisitor<T>, IExpressionVisitor<T>
    {
    }
}
