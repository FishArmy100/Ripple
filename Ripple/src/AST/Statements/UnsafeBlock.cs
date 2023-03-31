using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class UnsafeBlock : Statement
	{
		public readonly Token UnsafeToken;
		public readonly Token OpenBrace;
		public readonly List<Statement> Statements;
		public readonly Token CloseBrace;

		public UnsafeBlock(Token unsafeToken, Token openBrace, List<Statement> statements, Token closeBrace)
		{
			this.UnsafeToken = unsafeToken;
			this.OpenBrace = openBrace;
			this.Statements = statements;
			this.CloseBrace = closeBrace;
		}

		public override void Accept(IStatementVisitor visitor)
		{
			visitor.VisitUnsafeBlock(this);
		}

		public override T Accept<T>(IStatementVisitor<T> visitor)
		{
			return visitor.VisitUnsafeBlock(this);
		}

		public override TReturn Accept<TReturn, TArg>(IStatementVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitUnsafeBlock(this, arg);
		}

		public override void Accept<TArg>(IStatementVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitUnsafeBlock(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is UnsafeBlock unsafeBlock)
			{
				return UnsafeToken.Equals(unsafeBlock.UnsafeToken) && OpenBrace.Equals(unsafeBlock.OpenBrace) && Statements.SequenceEqual(unsafeBlock.Statements) && CloseBrace.Equals(unsafeBlock.CloseBrace);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(UnsafeToken);
			code.Add(OpenBrace);
			code.Add(Statements);
			code.Add(CloseBrace);
			return code.ToHashCode();
		}
	}
}
