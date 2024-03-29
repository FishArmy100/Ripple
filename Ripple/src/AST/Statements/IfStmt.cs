using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class IfStmt : Statement
	{
		public readonly Token IfTok;
		public readonly Token OpenParen;
		public readonly Expression Expr;
		public readonly Token CloseParen;
		public readonly Statement Body;
		public readonly Token? ElseToken;
		public readonly Option<Statement> ElseBody;

		public IfStmt(Token ifTok, Token openParen, Expression expr, Token closeParen, Statement body, Token? elseToken, Option<Statement> elseBody)
		{
			this.IfTok = ifTok;
			this.OpenParen = openParen;
			this.Expr = expr;
			this.CloseParen = closeParen;
			this.Body = body;
			this.ElseToken = elseToken;
			this.ElseBody = elseBody;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitIfStmt(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitIfStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitIfStmt(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitIfStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is IfStmt ifStmt)
			{
				return IfTok.Equals(ifStmt.IfTok) && OpenParen.Equals(ifStmt.OpenParen) && Expr.Equals(ifStmt.Expr) && CloseParen.Equals(ifStmt.CloseParen) && Body.Equals(ifStmt.Body) && ElseToken.Equals(ifStmt.ElseToken) && ElseBody.Equals(ifStmt.ElseBody);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(IfTok);
			code.Add(OpenParen);
			code.Add(Expr);
			code.Add(CloseParen);
			code.Add(Body);
			code.Add(ElseToken);
			code.Add(ElseBody);
			return code.ToHashCode();
		}
	}
}
