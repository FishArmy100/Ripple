using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	class SFuncPtr : SimplifiedType
	{
		public readonly bool IsMutable;
		public readonly List<SimplifiedType> Parameters;
		public readonly SimplifiedType Returned;

		public SFuncPtr(bool isMutable, List<SimplifiedType> parameters, SimplifiedType returned)
		{
			this.IsMutable = isMutable;
			this.Parameters = parameters;
			this.Returned = returned;
		}

		public override void Accept(ISimplifiedTypeVisitor visitor)
		{
			visitor.VisitSFuncPtr(this);
		}

		public override T Accept<T>(ISimplifiedTypeVisitor<T> visitor)
		{
			return visitor.VisitSFuncPtr(this);
		}

		public override TReturn Accept<TReturn, TArg>(ISimplifiedTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitSFuncPtr(this, arg);
		}

		public override void Accept<TArg>(ISimplifiedTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitSFuncPtr(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is SFuncPtr sFuncPtr)
			{
				return IsMutable.Equals(sFuncPtr.IsMutable) && Parameters.Equals(sFuncPtr.Parameters) && Returned.Equals(sFuncPtr.Returned);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IsMutable);
			code.Add(Parameters);
			code.Add(Returned);
			return code.ToHashCode();
		}
	}
}
