using System;


namespace Ripple.AST
{
	abstract class Statement
	{
		public abstract void Accept(IStatementVisitor iStatementVisitor);
		public abstract T Accept<T>(IStatementVisitor<T> iStatementVisitor);
	}
}
