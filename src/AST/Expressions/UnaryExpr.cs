using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class UnaryExpr : Expression
    {
        public readonly Expression Right;
        public readonly Token Operator;

        public UnaryExpr(Expression right, Token operatorToken)
        {
            Right = right;
            Operator = operatorToken;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitUnary(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitUnary(this);
        }
    }
}
