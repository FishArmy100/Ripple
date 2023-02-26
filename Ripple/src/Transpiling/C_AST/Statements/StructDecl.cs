using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class StructDecl : CStatement
	{
		public readonly string Name;
		public readonly List<StructMember> Members;

		public StructDecl(string name, List<StructMember> members)
		{
			this.Name = name;
			this.Members = members;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitStructDecl(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitStructDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitStructDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is StructDecl structDecl)
			{
				return Name.Equals(structDecl.Name) && Members.Equals(structDecl.Members);
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
