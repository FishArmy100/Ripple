using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Statements
{
	class TypedIfStmt : TypedStatement
	{
		public readonly TypedExpression Condition;
		public readonly TypedStatement Body;
		public readonly Option<TypedStatement> ElseBody;

		public TypedIfStmt(TypedExpression condition, TypedStatement body, Option<TypedStatement> elseBody)
		{
			this.Condition = condition;
			this.Body = body;
			this.ElseBody = elseBody;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedIfStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedIfStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedIfStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedIfStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedIfStmt typedIfStmt)
			{
				return Condition.Equals(typedIfStmt.Condition) && Body.Equals(typedIfStmt.Body) && ElseBody.Equals(typedIfStmt.ElseBody);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Condition);
			code.Add(Body);
			code.Add(ElseBody);
			return code.ToHashCode();
		}
	}
}
