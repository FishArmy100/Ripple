using System;


namespace Ripple.AST
{
	interface IExpressionVisitor
	{
		public abstract void VisitLiteral(Literal literal);
		public abstract void VisitGrouping(Grouping grouping);
		public abstract void VisitCall(Call call);
		public abstract void VisitUnary(Unary unary);
		public abstract void VisitBinary(Binary binary);
		public abstract void VisitIdentifier(Identifier identifier);
	}
}
