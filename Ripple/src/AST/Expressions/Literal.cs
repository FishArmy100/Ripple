using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class Literal : Expression
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
			return HashCode.Combine(Val);
		}
	}
}
