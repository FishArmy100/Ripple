using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    interface IExpressionVisitor<T>
    {
        public T VisitBinary(BinaryExpr binary);
        public T VisitLiteral(LiteralExpr literal);
        public T VisitUnary(UnaryExpr unary);
        public T VisitGrouping(GroupingExpr grouping);
        public T VisitAssignment(AssignmentExpr assignment);
        public T VisitIdentifier(IdentifierExpr variable);
        public T VisitCall(CallExpr call);
        public T VisitGet(GetExpr get);
        public T VisitNew(NewExpr newExpr);
    }

    interface IExpressionVisitor
    {
        public void VisitBinary(BinaryExpr binary);
        public void VisitLiteral(LiteralExpr literal);
        public void VisitUnary(UnaryExpr unary);
        public void VisitGrouping(GroupingExpr grouping);
        public void VisitAssignment(AssignmentExpr assignment);
        public void VisitIdentifier(IdentifierExpr variable);
        public void VisitCall(CallExpr call);
        public void VisitGet(GetExpr get);
        public void VisitNew(NewExpr newExpr);
    }
}
