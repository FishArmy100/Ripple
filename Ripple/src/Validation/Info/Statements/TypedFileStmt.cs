using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;
using System.Linq;


namespace Ripple.Validation.Info.Statements
{
	class TypedFileStmt : TypedStatement
	{
		public readonly List<TypedStatement> Statements;
		public readonly string RelativePath;

		public TypedFileStmt(List<TypedStatement> statements, string relativePath)
		{
			this.Statements = statements;
			this.RelativePath = relativePath;
		}

		public override void Accept(ITypedStatementVisitor visitor)
		{
			visitor.VisitTypedFileStmt(this);
		}

		public override T Accept<T>(ITypedStatementVisitor<T> visitor)
		{
			return visitor.VisitTypedFileStmt(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITypedStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTypedFileStmt(this, arg);
		}

		public override void Accept<TArg>(ITypedStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTypedFileStmt(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is TypedFileStmt typedFileStmt)
			{
				return Statements.SequenceEqual(typedFileStmt.Statements) && RelativePath.Equals(typedFileStmt.RelativePath);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Statements);
			code.Add(RelativePath);
			return code.ToHashCode();
		}
	}
}
