using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class ExprStmt : CStatement
	{
		public readonly CExpression Expression;

		public ExprStmt(CExpression expression)
		{
			this.Expression = expression;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitExprStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitExprStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitExprStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ExprStmt exprStmt)
			{
				return Expression.Equals(exprStmt.Expression);
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
