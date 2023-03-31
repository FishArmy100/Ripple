using System;
using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System.Linq;
using System.Linq;


namespace Ripple.Validation.Info.Expressions
{
	class TypedCast : TypedExpression
	{
		public readonly TypedExpression Casted;
		public readonly TypeInfo TypeToCastTo;

		public TypedCast(TypedExpression casted, TypeInfo typeToCastTo, TypeInfo returned) : base(returned)
		{
			this.Casted = casted;
			this.TypeToCastTo = typeToCastTo;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedCast(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedCast(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedCast(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedCast(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedCast typedCast)
			{
				return Casted.Equals(typedCast.Casted) && TypeToCastTo.Equals(typedCast.TypeToCastTo);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Casted);
			code.Add(TypeToCastTo);
			return code.ToHashCode();
		}
	}
}
