using System;
using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;


namespace Ripple.AST
{
	class InitializerList : Expression
	{
		public readonly Token OpenBrace;
		public readonly List<Expression> Expressions;
		public readonly Token CloseBrace;

		public InitializerList(Token openBrace, List<Expression> expressions, Token closeBrace)
		{
			this.OpenBrace = openBrace;
			this.Expressions = expressions;
			this.CloseBrace = closeBrace;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitInitializerList(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitInitializerList(this);
		}

		public override bool Equals(object other)
		{
			if(other is InitializerList initializerList)
			{
				return OpenBrace.Equals(initializerList.OpenBrace) && Expressions.Equals(initializerList.Expressions) && CloseBrace.Equals(initializerList.CloseBrace);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(OpenBrace);
			code.Add(Expressions);
			code.Add(CloseBrace);
			return code.ToHashCode();
		}
	}
}
