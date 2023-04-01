using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CWhileStmt : CStatement
	{
		public readonly CExpression Condition;
		public readonly CStatement Body;

		public CWhileStmt(CExpression condition, CStatement body)
		{
			this.Condition = condition;
			this.Body = body;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCWhileStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCWhileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCWhileStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCWhileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CWhileStmt cWhileStmt)
			{
				return Condition.Equals(cWhileStmt.Condition) && Body.Equals(cWhileStmt.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Condition);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
