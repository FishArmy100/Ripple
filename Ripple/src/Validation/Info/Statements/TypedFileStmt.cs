using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Statements
{
	class TypedFileStmt : TypedStatement
	{
		public readonly List<TypedStatement> Statements;
		public readonly string FilePath;

		public TypedFileStmt(List<TypedStatement> statements, string filePath)
		{
			this.Statements = statements;
			this.FilePath = filePath;
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
				return Statements.Equals(typedFileStmt.Statements) && FilePath.Equals(typedFileStmt.FilePath);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Statements);
			code.Add(FilePath);
			return code.ToHashCode();
		}
	}
}
