using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class Binary : Expression
	{
		public readonly Expression Left;
		public readonly Token Op;
		public readonly Expression Right;

		public Binary(Expression left, Token op, Expression right)
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
			HashCode code = new HashCode();
			code.Add(Left);
			code.Add(Op);
			code.Add(Right);
			return code.ToHashCode();
		}
	}
}
