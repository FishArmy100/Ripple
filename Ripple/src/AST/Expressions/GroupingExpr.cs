using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class GroupingExpr : Expression
    {
        public readonly Expression GroupedExpression;

        public GroupingExpr(Expression groupedExpression)
        {
            GroupedExpression = groupedExpression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitGrouping(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitGrouping(this);
        }
    }
}
