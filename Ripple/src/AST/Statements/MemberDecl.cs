using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class MemberDecl : Statement
	{
		public readonly Token? VisibilityToken;
		public readonly Statement Declaration;

		public MemberDecl(Token? visibilityToken, Statement declaration)
		{
			this.VisibilityToken = visibilityToken;
			this.Declaration = declaration;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitMemberDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitMemberDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitMemberDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitMemberDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is MemberDecl memberDecl)
			{
				return VisibilityToken.Equals(memberDecl.VisibilityToken) && Declaration.Equals(memberDecl.Declaration);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(VisibilityToken);
			code.Add(Declaration);
			return code.ToHashCode();
		}
	}
}
