using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Identifier : CExpression
	{
		public readonly string Id;

		public Identifier(string id)
		{
			this.Id = id;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitIdentifier(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitIdentifier(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitIdentifier(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Identifier identifier)
			{
				return Id.Equals(identifier.Id);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Id);
			return code.ToHashCode();
		}
	}
}
