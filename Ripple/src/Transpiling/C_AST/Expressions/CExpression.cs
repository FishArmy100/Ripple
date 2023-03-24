using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	abstract class CExpression
	{

		public CExpression()
		{
		}

		public abstract void Accept(ICExpressionVisitor iCExpressionVisitor);
		public abstract T Accept<T>(ICExpressionVisitor<T> iCExpressionVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> iCExpressionVisitor, TArg arg);
		public abstract void Accept<TArg>(ICExpressionVisitorWithArg<TArg> iCExpressionVisitor, TArg arg);
	}
}
