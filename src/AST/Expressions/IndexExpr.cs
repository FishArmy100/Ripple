using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class IndexExpr : Expression
    {
        public readonly Expression Indexee;
        public readonly List<Expression> Arguments;

        public IndexExpr(Expression indexee, List<Expression> arguments)
        {
            Indexee = indexee;
            Arguments = arguments;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitIndex(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitIndex(this);
        }
    }
}
