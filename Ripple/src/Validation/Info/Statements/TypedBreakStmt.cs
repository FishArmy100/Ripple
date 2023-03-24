using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	class TypedBreakStmt : TypedStatement
	{

		public TypedBreakStmt()
		{
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedBreakStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedBreakStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedBreakStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedBreakStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedBreakStmt typedBreakStmt)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeof(TypedBreakStmt).Name.GetHashCode();
		}
	}
}
