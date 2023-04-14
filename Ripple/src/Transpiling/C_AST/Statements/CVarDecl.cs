using System.Collections.Generic;
using Raucse;
using System;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	public class CVarDecl : CStatement
	{
		public readonly CType Type;
		public readonly string Name;
		public readonly Option<CExpression> Initializer;

		public CVarDecl(CType type, string name, Option<CExpression> initializer)
		{
			this.Type = type;
			this.Name = name;
			this.Initializer = initializer;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitCVarDecl(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitCVarDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitCVarDecl(this, arg);
		}

		public override void Accept<TArg>(ICStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitCVarDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is CVarDecl cVarDecl)
			{
				return Type.Equals(cVarDecl.Type) && Name.Equals(cVarDecl.Name) && Initializer.Equals(cVarDecl.Initializer);
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
