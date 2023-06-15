using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Lifetimes;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Values;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Expressions
{
	public abstract class TypedExpression
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
