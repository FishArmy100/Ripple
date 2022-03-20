using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class BinaryExpr : Expression
    {
        public readonly Expression Right;
        public readonly Token Operator;
        public readonly Expression Left;

        public BinaryExpr(Expression right, Token operatorToken, Expression left)
        {
            Right = right;
            Operator = operatorToken;
            Left = left;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitBinary(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitBinary(this);
        }
    }
}
