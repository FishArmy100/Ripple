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

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitVarDecl(this);
		}

		public override bool Equals(object other)
		{
			if(other is VarDecl varDecl)
			{
				return TypeName.Equals(varDecl.TypeName) && VarNames.Equals(varDecl.VarNames) && Equels.Equals(varDecl.Equels) && Expr.Equals(varDecl.Expr) && SemiColin.Equals(varDecl.SemiColin);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(TypeName, VarNames, Equels, Expr, SemiColin);
		}
	}
}
