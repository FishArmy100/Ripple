using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class IfStmt : CStatement
	{
		public readonly CExpression Condition;

		public IfStmt(CExpression condition)
		{
			this.Condition = condition;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitIfStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitIfStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitIfStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is IfStmt ifStmt)
			{
				return Condition.Equals(ifStmt.Condition);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Condition);
			return code.ToHashCode();
		}
	}
}
