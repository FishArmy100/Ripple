using System;


namespace ASTGeneration.Tests
{
	interface IExpressionVisitor
	{
		public abstract void VisitTest(Test test);
	}

	interface IExpressionVisitor<T>
	{
		public abstract T VisitTest(Test test);
	}
}
