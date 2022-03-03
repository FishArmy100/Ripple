using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    abstract class Statement
    {
        public class ExpressionStmt
        {
            public readonly Expression Expr;

            public ExpressionStmt(Expression expr)
            {
                Expr = expr;
            }
        }

        public interface IStatementVisitor
        {

        }

        public interface IStatementVisitor<T>
        {

        }
    }
}
