using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class GroupedType : TypeName
	{
		public readonly Token OpenParen;
		public readonly TypeName Type;
		public readonly Token CloseParen;

		public GroupedType(Token openParen, TypeName type, Token closeParen)
		{
			this.OpenParen = openParen;
			this.Type = type;
			this.CloseParen = closeParen;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitGroupedType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitGroupedType(this);
		}

		public override bool Equals(object other)
		{
			if(other is GroupedType groupedType)
			{
				return OpenParen.Equals(groupedType.OpenParen) && Type.Equals(groupedType.Type) && CloseParen.Equals(groupedType.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(OpenParen, Type, CloseParen);
		}
	}
}
