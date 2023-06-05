using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class DestructorDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly Token TildaToken;
		public readonly Token Identifier;
		public readonly Token OpenParen;
		public readonly Token CloseParen;
		public readonly BlockStmt Body;

		public DestructorDecl(Token? unsafeToken, Token tildaToken, Token identifier, Token openParen, Token closeParen, BlockStmt body)
		{
			this.UnsafeToken = unsafeToken;
			this.TildaToken = tildaToken;
			this.Identifier = identifier;
			this.OpenParen = openParen;
			this.CloseParen = closeParen;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitDestructorDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitDestructorDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitDestructorDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitDestructorDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is DestructorDecl destructorDecl)
			{
				return UnsafeToken.Equals(destructorDecl.UnsafeToken) && TildaToken.Equals(destructorDecl.TildaToken) && Identifier.Equals(destructorDecl.Identifier) && OpenParen.Equals(destructorDecl.OpenParen) && CloseParen.Equals(destructorDecl.CloseParen) && Body.Equals(destructorDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(TildaToken);
			code.Add(Identifier);
			code.Add(OpenParen);
			code.Add(CloseParen);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
