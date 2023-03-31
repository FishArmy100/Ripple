using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CIdentifier : CExpression
	{
		public readonly string Id;

		public CIdentifier(string id)
		{
			this.Id = id;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCIdentifier(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCIdentifier(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCIdentifier(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCIdentifier(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CIdentifier cIdentifier)
			{
				return Id.Equals(cIdentifier.Id);
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
