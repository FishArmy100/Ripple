using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Types
{
	class PointerInfo : TypeInfo
	{
		public readonly bool IsMutable;
		public readonly TypeInfo Contained;

		public PointerInfo(bool isMutable, TypeInfo contained)
		{
			this.IsMutable = isMutable;
			this.Contained = contained;
		}

		public override void Accept(ITypeInfoVisitor visitor)
		{
			visitor.VisitPointerInfo(this);
		}

		public override T Accept<T>(ITypeInfoVisitor<T> visitor)
		{
			return visitor.VisitPointerInfo(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitPointerInfo(this, arg);
		}

		public override void Accept<TArg>(ITypeInfoVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitPointerInfo(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is PointerInfo pointerInfo)
			{
				return IsMutable.Equals(pointerInfo.IsMutable) && Contained.Equals(pointerInfo.Contained);
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
