using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CBreakStmt : CStatement
	{

		public CBreakStmt()
		{
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCBreakStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCBreakStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCBreakStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCBreakStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CBreakStmt cBreakStmt)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeof(CBreakStmt).Name.GetHashCode();
		}
	}
}
