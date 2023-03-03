using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class ReturnStmt : CStatement
	{
		public readonly CExpression Expression;

		public ReturnStmt(CExpression expression)
		{
			this.Expression = expression;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitReturnStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitReturnStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitReturnStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ReturnStmt returnStmt)
			{
				return Expression.Equals(returnStmt.Expression);
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
