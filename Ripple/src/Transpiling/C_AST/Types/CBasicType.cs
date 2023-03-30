using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CBasicType : CType
	{
		public readonly string Name;
		public readonly bool IsConst;
		public readonly bool IsStruct;

		public CBasicType(string name, bool isConst, bool isStruct)
		{
			this.Name = name;
			this.IsConst = isConst;
			this.IsStruct = isStruct;
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

		public override void Accept<TArg>(ICTypeVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCBasicType(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CBasicType cBasicType)
			{
				return Name.Equals(cBasicType.Name) && IsConst.Equals(cBasicType.IsConst) && IsStruct.Equals(cBasicType.IsStruct);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			code.Add(IsConst);
			code.Add(IsStruct);
			return code.ToHashCode();
		}
	}
}
