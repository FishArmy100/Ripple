using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Statements
{
	abstract class TypedStatement
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
