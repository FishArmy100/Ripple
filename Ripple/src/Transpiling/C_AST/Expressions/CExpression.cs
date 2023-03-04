using System;


namespace Ripple.Transpiling.C_AST
{
	abstract class CExpression
	{
		public abstract void Accept(ICExpressionVisitor iCExpressionVisitor);
		public abstract T Accept<T>(ICExpressionVisitor<T> iCExpressionVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> iCExpressionVisitor, TArg arg);
		public abstract void Accept<TArg>(ICExpressionVisitorWithArg<TArg> iCExpressionVisitor, TArg arg);
	}
}
