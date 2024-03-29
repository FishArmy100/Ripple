using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CInitalizerList : CExpression
	{
		public readonly List<CExpression> Expressions;

		public CInitalizerList(List<CExpression> expressions)
		{
			this.Expressions = expressions;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCInitalizerList(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCInitalizerList(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCInitalizerList(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCInitalizerList(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CInitalizerList cInitalizerList)
			{
				return Expressions.SequenceEqual(cInitalizerList.Expressions);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Expressions);
			return code.ToHashCode();
		}
	}
}
