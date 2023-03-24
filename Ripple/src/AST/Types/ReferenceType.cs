using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.AST
{
	class ReferenceType : TypeName
	{
		public readonly TypeName BaseType;
		public readonly Token? MutToken;
		public readonly Token Ampersand;
		public readonly Token? Lifetime;

		public ReferenceType(TypeName baseType, Token? mutToken, Token ampersand, Token? lifetime)
		{
			this.BaseType = baseType;
			this.MutToken = mutToken;
			this.Ampersand = ampersand;
			this.Lifetime = lifetime;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitReferenceType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitReferenceType(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitReferenceType(this, arg);
		}

		public override void Accept<TArg>(ITypeNameVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitReferenceType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ReferenceType referenceType)
			{
				return BaseType.Equals(referenceType.BaseType) && MutToken.Equals(referenceType.MutToken) && Ampersand.Equals(referenceType.Ampersand) && Lifetime.Equals(referenceType.Lifetime);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(BaseType);
			code.Add(MutToken);
			code.Add(Ampersand);
			code.Add(Lifetime);
			return code.ToHashCode();
		}
	}
}
