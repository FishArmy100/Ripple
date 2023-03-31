using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	abstract class Statement
	{

		public Statement()
		{
		}

		public abstract void Accept(IStatementVisitor iStatementVisitor);
		public abstract T Accept<T>(IStatementVisitor<T> iStatementVisitor);
		public abstract TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> iStatementVisitor, TArg arg);
		public abstract void Accept<TArg>(IStatementVisitorWithArg<TArg> iStatementVisitor, TArg arg);
	}
}
