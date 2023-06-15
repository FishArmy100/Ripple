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


namespace Ripple.Validation.Info.Statements
{
	public abstract class TypedStatement
	{

		public TypedStatement()
		{
		}

		public abstract void Accept(ITypedStatementVisitor iTypedStatementVisitor);
		public abstract T Accept<T>(ITypedStatementVisitor<T> iTypedStatementVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> iTypedStatementVisitor, TArg arg);
		public abstract void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> iTypedStatementVisitor, TArg arg);
	}
}
