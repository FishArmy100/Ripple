using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CCall : CExpression
	{
		public readonly CExpression Callee;
		public readonly List<CExpression> Arguments;

		public CCall(CExpression callee, List<CExpression> arguments)
		{
			this.Callee = callee;
			this.Arguments = arguments;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCCall(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCCall(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCCall(this, arg);
		}

		public override void Accept<TArg>(ICExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCCall(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CCall cCall)
			{
				return Callee.Equals(cCall.Callee) && Arguments.SequenceEqual(cCall.Arguments);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Callee);
			code.Add(Arguments);
			return code.ToHashCode();
		}
	}
}
