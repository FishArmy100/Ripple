using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class FuncPtr : CType
	{
		public readonly CType Returned;
		public readonly List<CType> Parameters;

		public FuncPtr(CType returned, List<CType> parameters)
		{
			this.Returned = returned;
			this.Parameters = parameters;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitFuncPtr(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitFuncPtr(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFuncPtr(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FuncPtr funcPtr)
			{
				return Returned.Equals(funcPtr.Returned) && Parameters.Equals(funcPtr.Parameters);
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
