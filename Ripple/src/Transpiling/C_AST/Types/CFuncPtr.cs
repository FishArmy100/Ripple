using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CFuncPtr : CType
	{
		public readonly CType Returned;
		public readonly List<CType> Parameters;

		public CFuncPtr(CType returned, List<CType> parameters)
		{
			this.Returned = returned;
			this.Parameters = parameters;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitCFuncPtr(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitCFuncPtr(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCFuncPtr(this, arg);
		}

		public override void Accept<TArg>(ICTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCFuncPtr(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CFuncPtr cFuncPtr)
			{
				return Returned.Equals(cFuncPtr.Returned) && Parameters.SequenceEqual(cFuncPtr.Parameters);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Returned);
			code.Add(Parameters);
			return code.ToHashCode();
		}
	}
}
