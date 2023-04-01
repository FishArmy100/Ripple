using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CReturnStmt : CStatement
	{
		public readonly Option<CExpression> Expression;

		public CReturnStmt(Option<CExpression> expression)
		{
			this.Expression = expression;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCReturnStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCReturnStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCReturnStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCReturnStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CReturnStmt cReturnStmt)
			{
				return Expression.Equals(cReturnStmt.Expression);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expression);
			return code.ToHashCode();
		}
	}
}
