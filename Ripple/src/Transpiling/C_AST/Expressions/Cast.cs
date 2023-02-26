using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Cast : CExpression
	{
		public readonly CExpression Castee;
		public readonly CType Type;

		public Cast(CExpression castee, CType type)
		{
			this.Castee = castee;
			this.Type = type;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCast(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCast(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCast(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Cast cast)
			{
				return Castee.Equals(cast.Castee) && Type.Equals(cast.Type);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Castee);
			code.Add(Type);
			return code.ToHashCode();
		}
	}
}
