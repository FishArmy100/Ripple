using System;


namespace AST
{
	abstract class Expression
	{
		public abstract void Accept(IExpressionVisitor iExpressionVisitor);
	}
}
