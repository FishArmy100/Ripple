using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Expressions
{
	class TypedLiteral : TypedExpression
	{
		public readonly string Value;
		public readonly TokenType Type;

		public TypedLiteral(string value, TokenType type, TypeInfo returned) : base(returned)
		{
			this.Value = value;
			this.Type = type;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedLiteral(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedLiteral(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedLiteral(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedLiteral(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedLiteral typedLiteral)
			{
				return Value.Equals(typedLiteral.Value) && Type.Equals(typedLiteral.Type);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Value);
			code.Add(Type);
			return code.ToHashCode();
		}
	}
}
