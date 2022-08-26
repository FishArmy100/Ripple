using System;
using System.Collections.Generic;
using Ripple.Lexing;


namespace Ripple.AST
{
	class Identifier : Expression
	{

		public readonly Token Name;

		public Identifier(Token name)
		{
			this.Name = name;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitIdentifier(this);
		}
	}
}
