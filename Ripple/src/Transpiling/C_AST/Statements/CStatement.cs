using System;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	abstract class CStatement
	{

		public CStatement()
		{
		}

		public abstract void Accept(ICStatementVisitor iCStatementVisitor);
		public abstract T Accept<T>(ICStatementVisitor<T> iCStatementVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> iCStatementVisitor, TArg arg);
		public abstract void Accept<TArg>(ICStatementVisitorWithArg<TArg> iCStatementVisitor, TArg arg);
	}
}
