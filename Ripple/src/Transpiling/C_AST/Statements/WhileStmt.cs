using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class WhileStmt : CStatement
	{
		public readonly CExpression Condition;

		public WhileStmt(CExpression condition)
		{
			this.Condition = condition;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitWhileStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitWhileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitWhileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is WhileStmt whileStmt)
			{
				return Condition.Equals(whileStmt.Condition);
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
