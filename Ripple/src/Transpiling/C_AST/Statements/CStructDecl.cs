using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;
using System.Linq;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	class CStructDecl : CStatement
	{
		public readonly string Name;

		public CStructDecl(string name)
		{
			this.Name = name;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCStructDecl(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCStructDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCStructDecl(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCStructDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CStructDecl cStructDecl)
			{
				return Name.Equals(cStructDecl.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Name);
			return code.ToHashCode();
		}
	}
}
