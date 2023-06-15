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


namespace Ripple.Validation.Info.Expressions
{
	public class TypedBinary : TypedExpression
	{
		public readonly TypedExpression Left;
		public readonly TokenType Op;
		public readonly TypedExpression Right;

		public TypedBinary(TypedExpression left, TokenType op, TypedExpression right, TypeInfo returned) : base(returned)
		{
			this.Left = left;
			this.Op = op;
			this.Right = right;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedBinary(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedBinary(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedBinary(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedBinary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedBinary typedBinary)
			{
				return Left.Equals(typedBinary.Left) && Op.Equals(typedBinary.Op) && Right.Equals(typedBinary.Right);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Left);
			code.Add(Op);
			code.Add(Right);
			return code.ToHashCode();
		}
	}
}
