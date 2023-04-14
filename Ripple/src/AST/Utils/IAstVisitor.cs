using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST.Utils
{
    public interface IAstVisitor : IExpressionVisitor, IStatementVisitor, ITypeNameVisitor
    {
    }

    public interface IAstVisitor<T> : IExpressionVisitor<T>, IStatementVisitor<T> 
    {
    }
}
