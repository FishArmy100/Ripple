using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Statements
{
	class TypedExprStmt : TypedStatement
	{
		public readonly TypedExpression Expression;

		public TypedExprStmt(TypedExpression expression)
		{
			this.Expression = expression;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedExprStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedExprStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedExprStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedExprStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedExprStmt typedExprStmt)
			{
				return Expression.Equals(typedExprStmt.Expression);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expression);
			return code.ToHashCode();
		}
	}
}
