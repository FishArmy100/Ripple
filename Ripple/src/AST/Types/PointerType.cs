using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class PointerType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token? MutToken;
		public readonly Token Star;

		public PointerType(TypeName baseType, Token? mutToken, Token star)
		{
			this.BaseType = baseType;
			this.MutToken = mutToken;
			this.Star = star;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitPointerType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitPointerType(this);
		}

		public override bool Equals(object other)
		{
			if(other is PointerType pointerType)
			{
				return BaseType.Equals(pointerType.BaseType) && MutToken.Equals(pointerType.MutToken) && Star.Equals(pointerType.Star);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(BaseType, MutToken, Star);
		}
	}
}