using System;
using System.Collections.Generic;
using Ripple.Lexing;


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
	}
}