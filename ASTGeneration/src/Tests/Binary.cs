using System;


namespace ASTGeneration.Tests
{
	class Binary : Expression
	{
		public readonly Expression Left;
		public readonly char Op;
		public readonly Expression Right;

		public Binary(Expression left, char op, Expression right)
		{
			this.Left = left;
			this.Op = op;
			this.Right = right;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitBinary(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitBinary(this);
		}

		public override bool Equals(object other)
		{
			if(other is Binary binary)
			{
				return Left.Equals(binary.Left) && Op.Equals(binary.Op) && Right.Equals(binary.Right);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Left, Op, Right);
		}
	}
}
