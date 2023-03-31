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
	class TypedUnary : TypedExpression
	{
		public readonly TypedExpression Operand;
		public readonly TokenType Op;

		public TypedUnary(TypedExpression operand, TokenType op, TypeInfo returned) : base(returned)
		{
			this.Operand = operand;
			this.Op = op;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedUnary(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedUnary(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedUnary(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedUnary(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedUnary typedUnary)
			{
				return Operand.Equals(typedUnary.Operand) && Op.Equals(typedUnary.Op);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Operand);
			code.Add(Op);
			return code.ToHashCode();
		}
	}
}
