using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	class SArray : SimplifiedType
	{
		public readonly bool IsMutable;
		public readonly SimplifiedType Contained;
		public readonly int Size;

		public SArray(bool isMutable, SimplifiedType contained, int size)
		{
			this.IsMutable = isMutable;
			this.Contained = contained;
			this.Size = size;
		}

		public override void Accept(ISimplifiedTypeVisitor visitor)
		{
			visitor.VisitSArray(this);
		}

		public override T Accept<T>(ISimplifiedTypeVisitor<T> visitor)
		{
			return visitor.VisitSArray(this);
		}

		public override TReturn Accept<TReturn, TArg>(ISimplifiedTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSArray(this, arg);
		}

		public override void Accept<TArg>(ISimplifiedTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitSArray(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SArray sArray)
			{
				return IsMutable.Equals(sArray.IsMutable) && Contained.Equals(sArray.Contained) && Size.Equals(sArray.Size);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(Contained);
			code.Add(Size);
			return code.ToHashCode();
		}
	}
}
