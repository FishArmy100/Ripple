using System;
using System.Collections.Generic;
using Ripple.Utils;


namespace Ripple.Transpiling.C_AST
{
	class FuncDecl : CStatement
	{
		public readonly CType Returned;
		public readonly string Name;
		public readonly List<FuncParam> Parameters;
		public readonly BlockStmt Body;

		public FuncDecl(CType returned, string name, List<FuncParam> parameters, BlockStmt body)
		{
			this.Returned = returned;
			this.Name = name;
			this.Parameters = parameters;
			this.Body = body;
		}

		public override void Accept(ICStatementVisitor visitor)
		{
			visitor.VisitFuncDecl(this);
		}

		public override T Accept<T>(ICStatementVisitor<T> visitor)
		{
			return visitor.VisitFuncDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ICStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFuncDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FuncDecl funcDecl)
			{
				return Returned.Equals(funcDecl.Returned) && Name.Equals(funcDecl.Name) && Parameters.Equals(funcDecl.Parameters) && Body.Equals(funcDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Returned);
			code.Add(Name);
			code.Add(Parameters);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
