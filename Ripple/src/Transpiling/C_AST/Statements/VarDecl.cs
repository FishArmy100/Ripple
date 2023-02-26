using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class VarDecl : CStatement
	{
		public readonly CType Type;
		public readonly string Name;
		public readonly Option<CExpression> Initializer;

		public VarDecl(CType type, string name, Option<CExpression> initializer)
		{
			this.Type = type;
			this.Name = name;
			this.Initializer = initializer;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitVarDecl(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitVarDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitVarDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is VarDecl varDecl)
			{
				return Type.Equals(varDecl.Type) && Name.Equals(varDecl.Name) && Initializer.Equals(varDecl.Initializer);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			code.Add(Name);
			code.Add(Initializer);
			return code.ToHashCode();
		}
	}
}
