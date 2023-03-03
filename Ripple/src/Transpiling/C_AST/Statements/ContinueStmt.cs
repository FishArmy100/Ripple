using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class ContinueStmt : CStatement
	{

		public ContinueStmt()
		{
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitContinueStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitContinueStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitContinueStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ContinueStmt continueStmt)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeof(ContinueStmt).Name.GetHashCode();
		}
	}
}
