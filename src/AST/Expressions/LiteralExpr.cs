using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class LiteralExpr : Expression
    {
        public readonly Token Value;

        public LiteralExpr(Token value)
        {
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLiteral(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }
    }
}
