using System;


namespace Ripple.AST
{
	abstract class Expression
	{
		public abstract void Accept(IExpressionVisitor iExpressionVisitor);
		public abstract T Accept<T>(IExpressionVisitor<T> iExpressionVisitor);
		public abstract TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> iExpressionVisitor, TArg arg);
	}
}
