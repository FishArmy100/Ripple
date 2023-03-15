using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Expressions
{
	class TypedIdentifier : TypedExpression
	{
		public readonly string Name;
		public readonly Either<FunctionInfo, VariableInfo>;

		public TypedIdentifier(string name, Either<FunctionInfo, variableInfo>, TypeInfo returned) : base(returned)
		{
			this.Name = name;
			this.VariableInfo> = variableInfo>;
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
				return Name.Equals(typedIdentifier.Name) && VariableInfo>.Equals(typedIdentifier.VariableInfo>);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			code.Add(VariableInfo>);
			return code.ToHashCode();
		}
	}
}
