using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class Literal : Expression
	{
		public readonly Token Val;

		public Literal(Token val)
		{
			this.Val = val;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitLiteral(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitLiteral(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitLiteral(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitLiteral(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Literal literal)
			{
				return Val.Equals(literal.Val);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Val);
			return code.ToHashCode();
		}
	}
}
