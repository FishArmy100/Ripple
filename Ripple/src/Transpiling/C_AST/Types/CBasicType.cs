using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class CBasicType : CType
	{
		public readonly string Name;
		public readonly bool IsConst;

		public CBasicType(string name, bool isConst)
		{
			this.Name = name;
			this.IsConst = isConst;
		}

		public override void Accept(ICTypeVisitor visitor)
		{
			visitor.VisitCBasicType(this);
		}

		public override T Accept<T>(ICTypeVisitor<T> visitor)
		{
			return visitor.VisitCBasicType(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCBasicType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CBasicType cBasicType)
			{
				return Name.Equals(cBasicType.Name) && IsConst.Equals(cBasicType.IsConst);
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
