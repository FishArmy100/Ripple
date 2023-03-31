using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CTypeDefStmt : CStatement
	{
		public readonly CType Type;
		public readonly string Name;

		public CTypeDefStmt(CType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCTypeDefStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCTypeDefStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCTypeDefStmt(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCTypeDefStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CTypeDefStmt cTypeDefStmt)
			{
				return Type.Equals(cTypeDefStmt.Type) && Name.Equals(cTypeDefStmt.Name);
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
