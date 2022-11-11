using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class ReferenceType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token? MutToken;
		public readonly Token Ampersand;

		public ReferenceType(TypeName baseType, Token? mutToken, Token ampersand)
		{
			this.BaseType = baseType;
			this.MutToken = mutToken;
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
				return BaseType.Equals(referenceType.BaseType) && MutToken.Equals(referenceType.MutToken) && Ampersand.Equals(referenceType.Ampersand);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BaseType);
			code.Add(MutToken);
			code.Add(Ampersand);
			return code.ToHashCode();
		}
	}
}
