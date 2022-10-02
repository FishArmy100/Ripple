using System;


namespace ASTGeneration.Tests
{
	interface IExpressionVisitor
	{
		public abstract void VisitLiteral(Literal literal);
		public abstract void VisitBinary(Binary binary);
		public abstract void VisitUnary(Unary unary);
	}

	interface IExpressionVisitor<T>
	{
		public abstract T VisitLiteral(Literal literal);
		public abstract T VisitBinary(Binary binary);
		public abstract T VisitUnary(Unary unary);
	}
}
