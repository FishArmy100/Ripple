using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class ReferenceType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token Ampersand;

		public ReferenceType(TypeName baseType, Token ampersand)
		{
			this.BaseType = baseType;
			this.Ampersand = ampersand;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitReferenceType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitReferenceType(this);
		}

		public override bool Equals(object other)
		{
			if(other is ReferenceType referenceType)
			{
				return BaseType.Equals(referenceType.BaseType) && Ampersand.Equals(referenceType.Ampersand);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(BaseType, Ampersand);
		}
	}
}
