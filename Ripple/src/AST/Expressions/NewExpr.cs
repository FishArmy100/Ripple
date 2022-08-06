using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class NewExpr : Expression
    {
        public readonly Token Keyword;
        public readonly RippleType Type;
        public readonly List<Expression> Arguments;

        public NewExpr(Token keyword, RippleType type, List<Expression> parameters)
        {
            Keyword = keyword;
            Type = type;
            this.Arguments = parameters;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNew(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitNew(this);
        }
    }
}
