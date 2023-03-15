using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Statements
{
	class TypedContinueStmt : TypedStatement
	{

		public TypedContinueStmt()
		{
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedContinueStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedContinueStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedContinueStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedContinueStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedContinueStmt typedContinueStmt)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeof(TypedContinueStmt).Name.GetHashCode();
		}
	}
}
