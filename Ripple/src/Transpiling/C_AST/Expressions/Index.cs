using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Index : CExpression
	{
		public readonly CExpression Indexee;
		public readonly CExpression Argument;

		public Index(CExpression indexee, CExpression argument)
		{
			this.Indexee = indexee;
			this.Argument = argument;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitIndex(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitIndex(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitIndex(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Index index)
			{
				return Indexee.Equals(index.Indexee) && Argument.Equals(index.Argument);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Indexee);
			code.Add(Argument);
			return code.ToHashCode();
		}
	}
}
