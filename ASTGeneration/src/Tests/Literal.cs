using System;


namespace ASTGeneration.Tests
{
	class Literal : Expression
	{
		public readonly string LiteralValue;

		public Literal(string literalValue)
		{
			this.LiteralValue = literalValue;
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
				return LiteralValue.Equals(literal.LiteralValue);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(LiteralValue);
		}
	}
}
