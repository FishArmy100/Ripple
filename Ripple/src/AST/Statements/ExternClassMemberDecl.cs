using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ExternClassMemberDecl : Statement
	{
		public readonly Token Visibility;
		public readonly Token? UnsafeToken;
		public readonly TypeName Type;
		public readonly Token Name;
		public readonly Token SemiColon;

		public ExternClassMemberDecl(Token visibility, Token? unsafeToken, TypeName type, Token name, Token semiColon)
		{
			this.Visibility = visibility;
			this.UnsafeToken = unsafeToken;
			this.Type = type;
			this.Name = name;
			this.SemiColon = semiColon;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitExternClassMemberDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitExternClassMemberDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitExternClassMemberDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitExternClassMemberDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ExternClassMemberDecl externClassMemberDecl)
			{
				return Visibility.Equals(externClassMemberDecl.Visibility) && UnsafeToken.Equals(externClassMemberDecl.UnsafeToken) && Type.Equals(externClassMemberDecl.Type) && Name.Equals(externClassMemberDecl.Name) && SemiColon.Equals(externClassMemberDecl.SemiColon);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Visibility);
			code.Add(UnsafeToken);
			code.Add(Type);
			code.Add(Name);
			code.Add(SemiColon);
			return code.ToHashCode();
		}
	}
}
