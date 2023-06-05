using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ExternClassDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly Token ExternToken;
		public readonly Token Specifier;
		public readonly Token ClassToken;
		public readonly Token Name;
		public readonly Token OpenBrace;
		public readonly List<ExternClassMemberDecl> Members;
		public readonly Token CloseBrace;

		public ExternClassDecl(Token? unsafeToken, Token externToken, Token specifier, Token classToken, Token name, Token openBrace, List<ExternClassMemberDecl> members, Token closeBrace)
		{
			this.UnsafeToken = unsafeToken;
			this.ExternToken = externToken;
			this.Specifier = specifier;
			this.ClassToken = classToken;
			this.Name = name;
			this.OpenBrace = openBrace;
			this.Members = members;
			this.CloseBrace = closeBrace;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitExternClassDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitExternClassDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitExternClassDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitExternClassDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ExternClassDecl externClassDecl)
			{
				return UnsafeToken.Equals(externClassDecl.UnsafeToken) && ExternToken.Equals(externClassDecl.ExternToken) && Specifier.Equals(externClassDecl.Specifier) && ClassToken.Equals(externClassDecl.ClassToken) && Name.Equals(externClassDecl.Name) && OpenBrace.Equals(externClassDecl.OpenBrace) && Members.SequenceEqual(externClassDecl.Members) && CloseBrace.Equals(externClassDecl.CloseBrace);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(ExternToken);
			code.Add(Specifier);
			code.Add(ClassToken);
			code.Add(Name);
			code.Add(OpenBrace);
			code.Add(Members);
			code.Add(CloseBrace);
			return code.ToHashCode();
		}
	}
}
