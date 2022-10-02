using System;

namespace ASTGeneration.Tests
{
	class Unary : Expression
	{
		public readonly Expression Operand;
		public readonly char Op;

		public Unary(Expression operand, char op)
		{
			this.Operand = operand;
			this.Op = op;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitUnary(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitUnary(this);
		}

		public override bool Equals(object other)
		{
			if(other is Unary unary)
			{
				return Operand.Equals(unary.Operand) && Op.Equals(unary.Op);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Operand, Op);
		}
	}
}
