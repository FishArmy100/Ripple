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

	interface IExpressionVisitor<T>
	{
		public abstract T VisitLiteral(Literal literal);
		public abstract T VisitGrouping(Grouping grouping);
		public abstract T VisitCall(Call call);
		public abstract T VisitUnary(Unary unary);
		public abstract T VisitBinary(Binary binary);
		public abstract T VisitIdentifier(Identifier identifier);
	}
}
