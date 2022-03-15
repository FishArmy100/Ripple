using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class AssignmentExpr : Expression
    {
        public readonly Token Name;
        public readonly Expression Value;

        public AssignmentExpr(Token name, Expression value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitAssignment(this);
        }
    }
}
