using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CCast : CExpression
	{
		public readonly CExpression Castee;
		public readonly CType Type;

		public CCast(CExpression castee, CType type)
		{
			this.Castee = castee;
			this.Type = type;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCCast(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCCast(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCCast(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCCast(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CCast cCast)
			{
				return Castee.Equals(cCast.Castee) && Type.Equals(cCast.Type);
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
