using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CStructMember : CStatement
	{
		public readonly CType Type;
		public readonly string Name;

		public CStructMember(CType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCStructMember(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCStructMember(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCStructMember(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCStructMember(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CStructMember cStructMember)
			{
				return Type.Equals(cStructMember.Type) && Name.Equals(cStructMember.Name);
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
