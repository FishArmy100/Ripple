using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Call : CExpression
	{
		public readonly CExpression Callee;
		public readonly List<CExpression> Arguments;

		public Call(CExpression callee, List<CExpression> arguments)
		{
			this.Callee = callee;
			this.Arguments = arguments;
		}

		public override void Accept(ICExpressionVisitor visitor)
		{
			visitor.VisitCall(this);
		}

		public override T Accept<T>(ICExpressionVisitor<T> visitor)
		{
			return visitor.VisitCall(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCall(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Call call)
			{
				return Callee.Equals(call.Callee) && Arguments.Equals(call.Arguments);
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
