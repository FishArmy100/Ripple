using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST.Info.Types
{
	class ReferenceInfo : TypeInfo
	{
		public readonly bool IsMutable;
		public readonly TypeInfo Contained;
		public readonly Option<ReferenceLifetime> Lifetime;

		public ReferenceInfo(bool isMutable, TypeInfo contained, Option<ReferenceLifetime> lifetime)
		{
			this.IsMutable = isMutable;
			this.Contained = contained;
			this.Lifetime = lifetime;
		}

		public override void Accept(ITypeInfoVisitor visitor)
		{
			visitor.VisitReferenceInfo(this);
		}

		public override T Accept<T>(ITypeInfoVisitor<T> visitor)
		{
			return visitor.VisitReferenceInfo(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitReferenceInfo(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ReferenceInfo referenceInfo)
			{
				return IsMutable.Equals(referenceInfo.IsMutable) && Contained.Equals(referenceInfo.Contained) && Lifetime.Equals(referenceInfo.Lifetime);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(Contained);
			code.Add(Lifetime);
			return code.ToHashCode();
		}
	}
}
