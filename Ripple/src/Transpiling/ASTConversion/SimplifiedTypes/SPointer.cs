using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	class SPointer : SimplifiedType
	{
		public readonly bool IsMutable;
		public readonly SimplifiedType Contained;

		public SPointer(bool isMutable, SimplifiedType contained)
		{
			this.IsMutable = isMutable;
			this.Contained = contained;
		}

		public override void Accept(ISimplifiedTypeVisitor visitor)
		{
			visitor.VisitSPointer(this);
		}

		public override T Accept<T>(ISimplifiedTypeVisitor<T> visitor)
		{
			return visitor.VisitSPointer(this);
		}

		public override TReturn Accept<TReturn, TArg>(ISimplifiedTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSPointer(this, arg);
		}

		public override void Accept<TArg>(ISimplifiedTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitSPointer(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SPointer sPointer)
			{
				return IsMutable.Equals(sPointer.IsMutable) && Contained.Equals(sPointer.Contained);
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
