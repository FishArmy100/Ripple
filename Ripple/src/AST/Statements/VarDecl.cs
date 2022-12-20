using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class VarDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly TypeName Type;
		public readonly List<Token> VarNames;
		public readonly Token Equels;
		public readonly Expression Expr;
		public readonly Token SemiColin;

		public VarDecl(Token? unsafeToken, TypeName type, List<Token> varNames, Token equels, Expression expr, Token semiColin)
		{
			this.UnsafeToken = unsafeToken;
			this.Type = type;
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

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitVarDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is VarDecl varDecl)
			{
				return UnsafeToken.Equals(varDecl.UnsafeToken) && Type.Equals(varDecl.Type) && VarNames.Equals(varDecl.VarNames) && Equels.Equals(varDecl.Equels) && Expr.Equals(varDecl.Expr) && SemiColin.Equals(varDecl.SemiColin);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(Type);
			code.Add(VarNames);
			code.Add(Equels);
			code.Add(Expr);
			code.Add(SemiColin);
			return code.ToHashCode();
		}
	}
}
