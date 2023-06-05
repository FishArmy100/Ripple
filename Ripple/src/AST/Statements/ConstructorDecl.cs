using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class ConstructorDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly Token Identifier;
		public readonly Option<GenericParameters> GenericParameters;
		public readonly Parameters Parameters;
		public readonly BlockStmt Body;

		public ConstructorDecl(Token? unsafeToken, Token identifier, Option<GenericParameters> genericParameters, Parameters parameters, BlockStmt body)
		{
			this.UnsafeToken = unsafeToken;
			this.Identifier = identifier;
			this.GenericParameters = genericParameters;
			this.Parameters = parameters;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitConstructorDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitConstructorDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitConstructorDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitConstructorDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ConstructorDecl constructorDecl)
			{
				return UnsafeToken.Equals(constructorDecl.UnsafeToken) && Identifier.Equals(constructorDecl.Identifier) && GenericParameters.Equals(constructorDecl.GenericParameters) && Parameters.Equals(constructorDecl.Parameters) && Body.Equals(constructorDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(Identifier);
			code.Add(GenericParameters);
			code.Add(Parameters);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
