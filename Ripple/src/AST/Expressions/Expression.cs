using System;


namespace Ripple.AST
{
	abstract class Expression
	{
		public abstract void Accept(IExpressionVisitor iExpressionVisitor);
		public abstract T Accept<T>(IExpressionVisitor<T> iExpressionVisitor);
	}
}
