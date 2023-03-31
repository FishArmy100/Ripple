using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System.Linq;


namespace Ripple.AST
{
	abstract class Expression
	{

		public Expression()
		{
		}

		public abstract void Accept(IExpressionVisitor iExpressionVisitor);
		public abstract T Accept<T>(IExpressionVisitor<T> iExpressionVisitor);
		public abstract TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> iExpressionVisitor, TArg arg);
		public abstract void Accept<TArg>(IExpressionVisitorWithArg<TArg> iExpressionVisitor, TArg arg);
	}
}
