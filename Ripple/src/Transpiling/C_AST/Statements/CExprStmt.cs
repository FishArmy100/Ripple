using System;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CExprStmt : CStatement
	{
		public readonly CExpression Expression;

		public CExprStmt(CExpression expression)
		{
			this.Expression = expression;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCExprStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCExprStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCExprStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCExprStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CExprStmt cExprStmt)
			{
				return Expression.Equals(cExprStmt.Expression);
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
