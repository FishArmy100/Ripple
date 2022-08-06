using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class CallExpr : Expression
    {
        public readonly Expression Callee;
        public readonly List<Expression> Parameters;

        public CallExpr(Expression callee, List<Expression> parameters)
        {
            Callee = callee;
            Parameters = parameters;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitCall(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitCall(this);
        }
    }
}
