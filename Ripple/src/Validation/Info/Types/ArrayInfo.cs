using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;


namespace Ripple.Validation.Info.Types
{
	class ArrayInfo : TypeInfo
	{
		public readonly bool IsMutable;
		public readonly TypeInfo Contained;
		public readonly int Size;

		public ArrayInfo(bool isMutable, TypeInfo contained, int size)
		{
			this.IsMutable = isMutable;
			this.Contained = contained;
			this.Size = size;
		}

		public override void Accept(ITypeInfoVisitor visitor)
		{
			visitor.VisitArrayInfo(this);
		}

		public override T Accept<T>(ITypeInfoVisitor<T> visitor)
		{
			return visitor.VisitArrayInfo(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitArrayInfo(this, arg);
		}

		public override void Accept<TArg>(ITypeInfoVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitArrayInfo(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ArrayInfo arrayInfo)
			{
				return IsMutable.Equals(arrayInfo.IsMutable) && Contained.Equals(arrayInfo.Contained) && Size.Equals(arrayInfo.Size);
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
