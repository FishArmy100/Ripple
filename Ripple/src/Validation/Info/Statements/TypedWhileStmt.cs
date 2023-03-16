using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Statements
{
	class TypedWhileStmt : TypedStatement
	{
		public readonly TypedExpression Condition;
		public readonly TypedStatement Body;

		public TypedWhileStmt(TypedExpression condition, TypedStatement body)
		{
			this.Condition = condition;
			this.Body = body;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedWhileStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedWhileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedWhileStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedWhileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedWhileStmt typedWhileStmt)
			{
				return Condition.Equals(typedWhileStmt.Condition) && Body.Equals(typedWhileStmt.Body);
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
