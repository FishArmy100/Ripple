using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class FuncDecl : Statement
	{
		public readonly Token? UnsafeToken;
		public readonly Token? MutToken;
		public readonly Token FuncTok;
		public readonly Token Name;
		public readonly Option<GenericParameters> GenericParams;
		public readonly Parameters Param;
		public readonly Token Arrow;
		public readonly TypeName ReturnType;
		public readonly Option<WhereClause> WhereClause;
		public readonly BlockStmt Body;

		public FuncDecl(Token? unsafeToken, Token? mutToken, Token funcTok, Token name, Option<GenericParameters> genericParams, Parameters param, Token arrow, TypeName returnType, Option<WhereClause> whereClause, BlockStmt body)
		{
			this.UnsafeToken = unsafeToken;
			this.MutToken = mutToken;
			this.FuncTok = funcTok;
			this.Name = name;
			this.GenericParams = genericParams;
			this.Param = param;
			this.Arrow = arrow;
			this.ReturnType = returnType;
			this.WhereClause = whereClause;
			this.Body = body;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitFuncDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitFuncDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitFuncDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitFuncDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is FuncDecl funcDecl)
			{
				return UnsafeToken.Equals(funcDecl.UnsafeToken) && MutToken.Equals(funcDecl.MutToken) && FuncTok.Equals(funcDecl.FuncTok) && Name.Equals(funcDecl.Name) && GenericParams.Equals(funcDecl.GenericParams) && Param.Equals(funcDecl.Param) && Arrow.Equals(funcDecl.Arrow) && ReturnType.Equals(funcDecl.ReturnType) && WhereClause.Equals(funcDecl.WhereClause) && Body.Equals(funcDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(MutToken);
			code.Add(FuncTok);
			code.Add(Name);
			code.Add(GenericParams);
			code.Add(Param);
			code.Add(Arrow);
			code.Add(ReturnType);
			code.Add(WhereClause);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
