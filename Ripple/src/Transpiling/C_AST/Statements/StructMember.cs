using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class StructMember : CStatement
	{
		public readonly CType Type;
		public readonly string Name;

		public StructMember(CType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitStructMember(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitStructMember(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitStructMember(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is StructMember structMember)
			{
				return Type.Equals(structMember.Type) && Name.Equals(structMember.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			code.Add(Name);
			return code.ToHashCode();
		}
	}
}
