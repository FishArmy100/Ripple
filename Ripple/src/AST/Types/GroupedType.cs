using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class GroupedType : TypeName
	{
		public readonly Token OpenParen;
		public readonly TypeName GroupedType;
		public readonly Token CloseParen;

		public GroupedType(Token openParen, TypeName groupedType, Token closeParen)
		{
			this.OpenParen = openParen;
			this.GroupedType = groupedType;
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
				return OpenParen.Equals(groupedType.OpenParen) && GroupedType.Equals(groupedType.GroupedType) && CloseParen.Equals(groupedType.CloseParen);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(OpenParen, GroupedType, CloseParen);
		}
	}
}
