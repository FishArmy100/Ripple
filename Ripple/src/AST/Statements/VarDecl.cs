using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class VarDecl : Statement
	{

		public readonly Token TypeName;
		public readonly List<Token> VarNames;
		public readonly Token Equels;
		public readonly Expression Expr;
		public readonly Token SemiColin;

		public VarDecl(Token typeName, List<Token> varNames, Token equels, Expression expr, Token semiColin)
		{
			this.TypeName = typeName;
			this.VarNames = varNames;
			this.Equels = equels;
			this.Expr = expr;
			this.SemiColin = semiColin;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitVarDecl(this);
		}
	}
}
