using System;


namespace AST
{
	struct Token { }

	class Term : Expression
	{

		public readonly Token Left;
		public readonly Token Op;
		public readonly Token Right;

		public Term(Token left, Token op, Token right)
		{
			this.Left = left;
			this.Op = op;
			this.Right = right;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitTerm(this);
		}
	}
}
