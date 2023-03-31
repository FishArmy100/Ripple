using System.Collections.Generic;
using Raucse;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	public class TypedFuncDecl : TypedStatement
	{
		public readonly FunctionInfo Info;
		public readonly TypedBlockStmt Body;

		public TypedFuncDecl(FunctionInfo info, TypedBlockStmt body)
		{
			this.Info = info;
			this.Body = body;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedFuncDecl(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedFuncDecl(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedFuncDecl(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedFuncDecl(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedFuncDecl typedFuncDecl)
			{
				return Info.Equals(typedFuncDecl.Info) && Body.Equals(typedFuncDecl.Body);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Info);
			code.Add(Body);
			return code.ToHashCode();
		}
	}
}
