using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Lifetimes;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Values;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Types
{
	public class ArrayInfo : TypeInfo
	{
		public readonly TypeInfo Contained;
		public readonly int Size;

		public ArrayInfo(TypeInfo contained, int size)
		{
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
				return Contained.Equals(arrayInfo.Contained) && Size.Equals(arrayInfo.Size);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Contained);
			code.Add(Size);
			return code.ToHashCode();
		}
	}
}
