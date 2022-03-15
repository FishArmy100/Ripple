using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class IdentifierExpr : Expression
    {
        public readonly Token Name;

        public IdentifierExpr(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitIdentifier(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitIdentifier(this);
        }
    }
}
