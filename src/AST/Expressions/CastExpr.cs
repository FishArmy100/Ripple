using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class CastExpr : Expression
    {
        public readonly Expression Right;
        public readonly RippleType Type;

        public CastExpr(Expression right, RippleType type)
        {
            Right = right;
            Type = type;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitCast(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitCast(this);
        }
    }
}
