using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CStructDef : CStatement
	{
		public readonly string Name;
		public readonly List<CStructMember> Members;

		public CStructDef(string name, List<CStructMember> members)
		{
			this.Name = name;
			this.Members = members;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCStructDef(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCStructDef(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCStructDef(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCStructDef(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CStructDef cStructDef)
			{
				return Name.Equals(cStructDef.Name) && Members.SequenceEqual(cStructDef.Members);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			code.Add(Members);
			return code.ToHashCode();
		}
	}
}
