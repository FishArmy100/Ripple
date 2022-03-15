using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class ExpressionStmt : Statement
    {
        public readonly Expression Expr;

        public ExpressionStmt(Expression expr)
        {
            Expr = expr;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitExpressionStmt(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
}
