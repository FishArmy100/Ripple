using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ClassDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly Token ClassToken;
		public readonly Token Name;
		public readonly Option<GenericParameters> GenericParameters;
		public readonly Token OpenBrace;
		public readonly List<Statement> Members;
		public readonly Token CloseBrace;

		public ClassDecl(Token? unsafeToken, Token classToken, Token name, Option<GenericParameters> genericParameters, Token openBrace, List<Statement> members, Token closeBrace)
		{
			this.UnsafeToken = unsafeToken;
			this.ClassToken = classToken;
			this.Name = name;
			this.GenericParameters = genericParameters;
			this.OpenBrace = openBrace;
			this.Members = members;
			this.CloseBrace = closeBrace;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitClassDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitClassDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitClassDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitClassDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ClassDecl classDecl)
			{
				return UnsafeToken.Equals(classDecl.UnsafeToken) && ClassToken.Equals(classDecl.ClassToken) && Name.Equals(classDecl.Name) && GenericParameters.Equals(classDecl.GenericParameters) && OpenBrace.Equals(classDecl.OpenBrace) && Members.SequenceEqual(classDecl.Members) && CloseBrace.Equals(classDecl.CloseBrace);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(ClassToken);
			code.Add(Name);
			code.Add(GenericParameters);
			code.Add(OpenBrace);
			code.Add(Members);
			code.Add(CloseBrace);
			return code.ToHashCode();
		}
	}
}
