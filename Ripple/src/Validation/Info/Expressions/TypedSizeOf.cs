using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System.Linq;
using System.Linq;


namespace Ripple.Validation.Info.Expressions
{
	class TypedSizeOf : TypedExpression
	{
		public readonly TypeInfo SizedType;

		public TypedSizeOf(TypeInfo sizedType, TypeInfo returned) : base(returned)
		{
			this.SizedType = sizedType;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedSizeOf(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedSizeOf(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedSizeOf(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedSizeOf(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedSizeOf typedSizeOf)
			{
				return SizedType.Equals(typedSizeOf.SizedType);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(SizedType);
			return code.ToHashCode();
		}
	}
}
