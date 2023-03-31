using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Expressions
{
	public class TypedIndex : TypedExpression
	{
		public readonly TypedExpression Indexee;
		public readonly TypedExpression Argument;

		public TypedIndex(TypedExpression indexee, TypedExpression argument, TypeInfo returned) : base(returned)
		{
			this.Indexee = indexee;
			this.Argument = argument;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedIndex(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedIndex(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedIndex(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedIndex(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedIndex typedIndex)
			{
				return Indexee.Equals(typedIndex.Indexee) && Argument.Equals(typedIndex.Argument);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Indexee);
			code.Add(Argument);
			return code.ToHashCode();
		}
	}
}
