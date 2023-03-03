using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class TypeDefStmt : CStatement
	{
		public readonly CType Type;
		public readonly string Name;

		public TypeDefStmt(CType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitTypeDefStmt(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitTypeDefStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypeDefStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypeDefStmt typeDefStmt)
			{
				return Type.Equals(typeDefStmt.Type) && Name.Equals(typeDefStmt.Name);
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
