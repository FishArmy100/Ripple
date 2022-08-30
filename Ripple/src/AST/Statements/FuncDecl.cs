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

		public FuncDecl(Token funcTok, Token name, Parameters param, Token arrow, Token returnType)
		{
			this.FuncTok = funcTok;
			this.Name = name;
			this.Param = param;
			this.Arrow = arrow;
			this.ReturnType = returnType;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitFuncDecl(this);
		}
	}
}
