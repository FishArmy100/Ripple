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

		public override bool Equals(object other)
		{
			if(other is Identifier identifier)
			{
				return Name.Equals(identifier.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			return code.ToHashCode();
		}
	}
}
