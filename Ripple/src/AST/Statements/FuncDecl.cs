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
		public readonly Token ReturnType;
		public readonly BlockStmt Body;

		public FuncDecl(Token funcTok, Token name, Parameters param, Token arrow, Token returnType, BlockStmt body)
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

	}
}
