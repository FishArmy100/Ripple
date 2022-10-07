using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class FuncDecl : Statement
	{
		public readonly Token FuncTok;
		public readonly Token Name;
		public readonly Parameters Param;
		public readonly Token Arrow;
		public readonly TypeName ReturnType;
		public readonly BlockStmt Body;

		public FuncDecl(Token funcTok, Token name, Parameters param, Token arrow, TypeName returnType, BlockStmt body)
		{
			this.FuncTok = funcTok;
			this.Name = name;
			this.Param = param;
			this.Arrow = arrow;
			this.ReturnType = returnType;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitFuncDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitFuncDecl(this);
		}

		public override bool Equals(object other)
		{
			if(other is FuncDecl funcDecl)
			{
				return FuncTok.Equals(funcDecl.FuncTok) && Name.Equals(funcDecl.Name) && Param.Equals(funcDecl.Param) && Arrow.Equals(funcDecl.Arrow) && ReturnType.Equals(funcDecl.ReturnType) && Body.Equals(funcDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(FuncTok, Name, Param, Arrow, ReturnType, Body);
		}
	}
}
