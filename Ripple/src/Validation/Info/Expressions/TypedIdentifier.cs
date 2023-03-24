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
	class TypedIdentifier : TypedExpression
	{
		public readonly string Name;
		public readonly Either<FunctionInfo,VariableInfo> Value;

		public TypedIdentifier(string name, Either<FunctionInfo,VariableInfo> value, TypeInfo returned) : base(returned)
		{
			this.Name = name;
			this.Value = value;
		}

		public override void Accept(ITypedExpressionVisitor visitor)
		{
			visitor.VisitTypedIdentifier(this);
		}

		public override T Accept<T>(ITypedExpressionVisitor<T> visitor)
		{
			return visitor.VisitTypedIdentifier(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedIdentifier(this, arg);
		}

		public override void Accept<TArg>(ITypedExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedIdentifier(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedIdentifier typedIdentifier)
			{
				return Name.Equals(typedIdentifier.Name) && Value.Equals(typedIdentifier.Value);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			code.Add(Value);
			return code.ToHashCode();
		}
	}
}
