using System;


namespace AST
{
	interface IExpressionVisitor
	{
		public abstract void VisitTerm(Term term);
	}
}
