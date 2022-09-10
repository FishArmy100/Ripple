using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


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

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitIdentifier(this);
		}

	}
}
