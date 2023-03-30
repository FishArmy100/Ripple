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
	class TypedReturnStmt : TypedStatement
	{
		public readonly Option<TypedExpression> Expression;

		public TypedReturnStmt(Option<TypedExpression> expression)
		{
			this.Expression = expression;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedReturnStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedReturnStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedReturnStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedReturnStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedReturnStmt typedReturnStmt)
			{
				return Expression.Equals(typedReturnStmt.Expression);
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
