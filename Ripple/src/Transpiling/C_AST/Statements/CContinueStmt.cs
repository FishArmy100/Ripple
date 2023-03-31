using System;
using System.Collections.Generic;
using Raucse;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CContinueStmt : CStatement
	{

		public CContinueStmt()
		{
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCContinueStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCContinueStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCContinueStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCContinueStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CContinueStmt cContinueStmt)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeof(CContinueStmt).Name.GetHashCode();
		}
	}
}
