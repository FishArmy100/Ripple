using System;


namespace Ripple.AST
{
	interface IExpressionVisitor
	{
		public abstract void VisitLiteral(Literal literal);
		public abstract void VisitGrouping(Grouping grouping);
		public abstract void VisitCall(Call call);
		public abstract void VisitIndex(Index index);
		public abstract void VisitCast(Cast cast);
		public abstract void VisitUnary(Unary unary);
		public abstract void VisitBinary(Binary binary);
		public abstract void VisitIdentifier(Identifier identifier);
		public abstract void VisitTypeExpression(TypeExpression typeExpression);
		public abstract void VisitInitializerList(InitializerList initializerList);
		public abstract void VisitSizeOf(SizeOf sizeOf);
	}

	interface IExpressionVisitor<T>
	{
		public abstract T VisitLiteral(Literal literal);
		public abstract T VisitGrouping(Grouping grouping);
		public abstract T VisitCall(Call call);
		public abstract T VisitIndex(Index index);
		public abstract T VisitCast(Cast cast);
		public abstract T VisitUnary(Unary unary);
		public abstract T VisitBinary(Binary binary);
		public abstract T VisitIdentifier(Identifier identifier);
		public abstract T VisitTypeExpression(TypeExpression typeExpression);
		public abstract T VisitInitializerList(InitializerList initializerList);
		public abstract T VisitSizeOf(SizeOf sizeOf);
	}
}
