using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class BreakStmt : CStatement
	{

		public BreakStmt()
		{
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitBreakStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitBreakStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBreakStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is BreakStmt breakStmt)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeof(BreakStmt).Name.GetHashCode();
		}
	}
}
