using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class MemberFunctionDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly Token FuncToken;
		public readonly Token NameToken;
		public readonly Option<GenericParameters> GenericParameters;
		public readonly MemberFunctionParameters Parameters;
		public readonly Token Arrow;
		public readonly TypeName ReturnType;
		public readonly Option<WhereClause> WhereClause;
		public readonly BlockStmt Body;

		public MemberFunctionDecl(Token? unsafeToken, Token funcToken, Token nameToken, Option<GenericParameters> genericParameters, MemberFunctionParameters parameters, Token arrow, TypeName returnType, Option<WhereClause> whereClause, BlockStmt body)
		{
			this.UnsafeToken = unsafeToken;
			this.FuncToken = funcToken;
			this.NameToken = nameToken;
			this.GenericParameters = genericParameters;
			this.Parameters = parameters;
			this.Arrow = arrow;
			this.ReturnType = returnType;
			this.WhereClause = whereClause;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitMemberFunctionDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitMemberFunctionDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitMemberFunctionDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitMemberFunctionDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is MemberFunctionDecl memberFunctionDecl)
			{
				return UnsafeToken.Equals(memberFunctionDecl.UnsafeToken) && FuncToken.Equals(memberFunctionDecl.FuncToken) && NameToken.Equals(memberFunctionDecl.NameToken) && GenericParameters.Equals(memberFunctionDecl.GenericParameters) && Parameters.Equals(memberFunctionDecl.Parameters) && Arrow.Equals(memberFunctionDecl.Arrow) && ReturnType.Equals(memberFunctionDecl.ReturnType) && WhereClause.Equals(memberFunctionDecl.WhereClause) && Body.Equals(memberFunctionDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(FuncToken);
			code.Add(NameToken);
			code.Add(GenericParameters);
			code.Add(Parameters);
			code.Add(Arrow);
			code.Add(ReturnType);
			code.Add(WhereClause);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
