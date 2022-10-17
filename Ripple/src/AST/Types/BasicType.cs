using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class BasicType : TypeName
	{
		public readonly Token? MutToken;
		public readonly Token Identifier;

		public BasicType(Token? mutToken, Token identifier)
		{
			this.MutToken = mutToken;
			this.Identifier = identifier;
		}

		public override void Accept(ITypeNameVisitor visitor)
		{
			visitor.VisitBasicType(this);
		}

		public override T Accept<T>(ITypeNameVisitor<T> visitor)
		{
			return visitor.VisitBasicType(this);
		}

		public override bool Equals(object other)
		{
			if(other is BasicType basicType)
			{
				return MutToken.Equals(basicType.MutToken) && Identifier.Equals(basicType.Identifier);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(MutToken, Identifier);
		}
	}
}
