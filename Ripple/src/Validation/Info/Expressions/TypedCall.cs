using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Expressions
{
	class TypedCall : TypedExpression
	{
		public readonly TypedExpression Callee;
		public readonly List<TypedExpression> Arguments;

		public TypedCall(TypedExpression callee, List<TypedExpression> arguments, TypeInfo returned) : base(returned)
		{
			this.Callee = callee;
			this.Arguments = arguments;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedCall(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedCall(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedCall(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedCall(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedCall typedCall)
			{
				return Callee.Equals(typedCall.Callee) && Arguments.Equals(typedCall.Arguments);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Callee);
			code.Add(Arguments);
			return code.ToHashCode();
		}
	}
}
