using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class Basic : CType
	{
		public readonly string Name;
		public readonly bool IsConst;

		public Basic(string name, bool isConst)
		{
			this.Name = name;
			this.IsConst = isConst;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitBasic(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitBasic(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitBasic(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Basic basic)
			{
				return Name.Equals(basic.Name) && IsConst.Equals(basic.IsConst);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			code.Add(IsConst);
			return code.ToHashCode();
		}
	}
}
