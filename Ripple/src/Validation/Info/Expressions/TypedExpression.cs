using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Expressions
{
	abstract class TypedExpression
	{
		public readonly TypeInfo Returned;

		public TypedExpression(TypeInfo returned)
		{
			this.Returned = returned;
		}

		public abstract void Accept(ITypedExpressionVisitor iTypedExpressionVisitor);
		public abstract T Accept<T>(ITypedExpressionVisitor<T> iTypedExpressionVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> iTypedExpressionVisitor, TArg arg);
		public abstract void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> iTypedExpressionVisitor, TArg arg);
	}
}
