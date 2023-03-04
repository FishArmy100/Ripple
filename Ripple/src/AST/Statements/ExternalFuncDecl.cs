using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;


namespace Ripple.AST
{
	class ExternalFuncDecl : Statement
	{
		public readonly Token ExternToken;
		public readonly Token Specifier;
		public readonly Token FuncToken;
		public readonly Token Name;
		public readonly Parameters Parameters;
		public readonly Token Arrow;
		public readonly TypeName ReturnType;
		public readonly Token SemiColon;

		public ExternalFuncDecl(Token externToken, Token specifier, Token funcToken, Token name, Parameters parameters, Token arrow, TypeName returnType, Token semiColon)
		{
			this.ExternToken = externToken;
			this.Specifier = specifier;
			this.FuncToken = funcToken;
			this.Name = name;
			this.Parameters = parameters;
			this.Arrow = arrow;
			this.ReturnType = returnType;
			this.SemiColon = semiColon;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitExternalFuncDecl(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitExternalFuncDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitExternalFuncDecl(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitExternalFuncDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is ExternalFuncDecl externalFuncDecl)
			{
				return ExternToken.Equals(externalFuncDecl.ExternToken) && Specifier.Equals(externalFuncDecl.Specifier) && FuncToken.Equals(externalFuncDecl.FuncToken) && Name.Equals(externalFuncDecl.Name) && Parameters.Equals(externalFuncDecl.Parameters) && Arrow.Equals(externalFuncDecl.Arrow) && ReturnType.Equals(externalFuncDecl.ReturnType) && SemiColon.Equals(externalFuncDecl.SemiColon);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(ExternToken);
			code.Add(Specifier);
			code.Add(FuncToken);
			code.Add(Name);
			code.Add(Parameters);
			code.Add(Arrow);
			code.Add(ReturnType);
			code.Add(SemiColon);
			return code.ToHashCode();
		}
	}
}
