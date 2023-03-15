using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Expressions
{
	class TypedInitalizerList : TypedExpression
	{
		public readonly List<TypedExpression> Expressions;

		public TypedInitalizerList(List<TypedExpression> expressions, TypeInfo returned) : base(returned)
		{
			this.Expressions = expressions;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedInitalizerList(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedInitalizerList(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedInitalizerList(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedInitalizerList(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedInitalizerList typedInitalizerList)
			{
				return Expressions.Equals(typedInitalizerList.Expressions);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expressions);
			return code.ToHashCode();
		}
	}
}
