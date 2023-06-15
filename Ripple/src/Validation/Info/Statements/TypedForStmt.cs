using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Lifetimes;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Values;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	public class TypedForStmt : TypedStatement
	{
		public readonly Option<TypedStatement> Initalizer;
		public readonly Option<TypedExpression> Condition;
		public readonly Option<TypedExpression> Iterator;
		public readonly TypedStatement Body;

		public TypedForStmt(Option<TypedStatement> initalizer, Option<TypedExpression> condition, Option<TypedExpression> iterator, TypedStatement body)
		{
			this.Initalizer = initalizer;
			this.Condition = condition;
			this.Iterator = iterator;
			this.Body = body;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedForStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedForStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedForStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedForStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedForStmt typedForStmt)
			{
				return Initalizer.Equals(typedForStmt.Initalizer) && Condition.Equals(typedForStmt.Condition) && Iterator.Equals(typedForStmt.Iterator) && Body.Equals(typedForStmt.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Initalizer);
			code.Add(Condition);
			code.Add(Iterator);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
