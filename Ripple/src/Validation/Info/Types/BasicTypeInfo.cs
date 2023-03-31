using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Types
{
	public class BasicTypeInfo : TypeInfo
	{
		public readonly bool IsMutable;
		public readonly string Name;

		public BasicTypeInfo(bool isMutable, string name)
		{
			this.IsMutable = isMutable;
			this.Name = name;
		}

		public override void Accept(ITypeInfoVisitor visitor)
		{
			visitor.VisitBasicTypeInfo(this);
		}

		public override T Accept<T>(ITypeInfoVisitor<T> visitor)
		{
			return visitor.VisitBasicTypeInfo(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBasicTypeInfo(this, arg);
		}

		public override void Accept<TArg>(ITypeInfoVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitBasicTypeInfo(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is BasicTypeInfo basicTypeInfo)
			{
				return IsMutable.Equals(basicTypeInfo.IsMutable) && Name.Equals(basicTypeInfo.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(Name);
			return code.ToHashCode();
		}
	}
}
