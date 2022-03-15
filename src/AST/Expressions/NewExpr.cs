using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class NewExpr : Expression
    {
        public Token Keyword;
        public Token Type;
        public List<Expression> Arguments;

        public NewExpr(Token keyword, Token type, List<Expression> parameters)
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
