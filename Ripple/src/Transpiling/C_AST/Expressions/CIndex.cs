using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CIndex : CExpression
	{
		public readonly CExpression Indexee;
		public readonly CExpression Argument;

		public CIndex(CExpression indexee, CExpression argument)
		{
			this.Indexee = indexee;
			this.Argument = argument;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCIndex(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCIndex(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCIndex(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCIndex(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CIndex cIndex)
			{
				return Indexee.Equals(cIndex.Indexee) && Argument.Equals(cIndex.Argument);
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
