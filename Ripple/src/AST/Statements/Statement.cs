using System;


namespace Ripple.AST
{
	abstract class Statement
	{
		public abstract void Accept(IStatementVisitor iStatementVisitor);
		public abstract T Accept<T>(IStatementVisitor<T> iStatementVisitor);
		public abstract TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> iStatementVisitor, TArg arg);
	}
}
