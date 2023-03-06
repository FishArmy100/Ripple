using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	class SBasicType : SimplifiedType
	{
		public readonly bool IsMutable;
		public readonly string Name;

		public SBasicType(bool isMutable, string name)
		{
			this.IsMutable = isMutable;
			this.Name = name;
		}

		public override void Accept(ISimplifiedTypeVisitor visitor)
		{
			visitor.VisitSBasicType(this);
		}

		public override T Accept<T>(ISimplifiedTypeVisitor<T> visitor)
		{
			return visitor.VisitSBasicType(this);
		}

		public override TReturn Accept<TReturn, TArg>(ISimplifiedTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSBasicType(this, arg);
		}

		public override void Accept<TArg>(ISimplifiedTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitSBasicType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SBasicType sBasicType)
			{
				return IsMutable.Equals(sBasicType.IsMutable) && Name.Equals(sBasicType.Name);
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
