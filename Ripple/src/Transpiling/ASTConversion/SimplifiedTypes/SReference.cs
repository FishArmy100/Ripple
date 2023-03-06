using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	class SReference : SimplifiedType
	{
		public readonly bool IsMutable;
		public readonly SimplifiedType Contained;

		public SReference(bool isMutable, SimplifiedType contained)
		{
			this.IsMutable = isMutable;
			this.Contained = contained;
		}

		public override void Accept(ISimplifiedTypeVisitor visitor)
		{
			visitor.VisitSReference(this);
		}

		public override T Accept<T>(ISimplifiedTypeVisitor<T> visitor)
		{
			return visitor.VisitSReference(this);
		}

		public override TReturn Accept<TReturn, TArg>(ISimplifiedTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSReference(this, arg);
		}

		public override void Accept<TArg>(ISimplifiedTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitSReference(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SReference sReference)
			{
				return IsMutable.Equals(sReference.IsMutable) && Contained.Equals(sReference.Contained);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(Contained);
			return code.ToHashCode();
		}
	}
}
