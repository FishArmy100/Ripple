using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class GetExpr : Expression
    {
        public readonly Expression Object;
        public readonly Token Name;

        public GetExpr(Expression obj, Token name)
        {
            Object = obj;
            Name = name;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitGet(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitGet(this);
        }
    }
}
