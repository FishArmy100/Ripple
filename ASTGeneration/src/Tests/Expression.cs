using System;


namespace ASTGeneration.Tests
{
	abstract class Expression
	{
		public abstract void Accept(IExpressionVisitor iExpressionVisitor);
		public abstract T Accept<T>(IExpressionVisitor<T> iExpressionVisitor);
	}
}
