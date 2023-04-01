
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Utils
{
    public interface IBasicASTVisitor : IStatementVisitor, IExpressionVisitor
    {
    }

    public interface IBasicASTVisitor<T> : IStatementVisitor<T>, IExpressionVisitor<T>
    {
    }
}
