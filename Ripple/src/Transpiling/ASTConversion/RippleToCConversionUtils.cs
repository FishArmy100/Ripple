using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.Lexing;
using Ripple.AST;

namespace Ripple.Transpiling.ASTConversion
{
	static class RippleToCConversionUtils
	{
		public static CBinaryOperator ToBinary(TokenType operatorType)
		{
			return operatorType switch
			{
				TokenType.Plus => CBinaryOperator.Plus,
				TokenType.Minus => CBinaryOperator.Minus,
				TokenType.Star => CBinaryOperator.Star,
				TokenType.Slash => CBinaryOperator.Divide,
				TokenType.Equal => CBinaryOperator.Assign,
				TokenType.Mod => CBinaryOperator.Mod,
				TokenType.EqualEqual => CBinaryOperator.EqualEqual,
				TokenType.BangEqual => CBinaryOperator.BangEqual,
				TokenType.GreaterThan => CBinaryOperator.GreaterThan,
				TokenType.LessThan => CBinaryOperator.LessThan,
				TokenType.GreaterThanEqual => CBinaryOperator.GreaterThanEqual,
				TokenType.LessThanEqual => CBinaryOperator.LessThanEqual,
				TokenType.AmpersandAmpersand => CBinaryOperator.AnpersandAnpersand,
				TokenType.PipePipe => CBinaryOperator.PipePipe,
				_ => throw new ArgumentException("Invalid binary operator type for transpiling " + operatorType)
			};
		}

		public static CUnaryOperator ToUnary(TokenType operatorType)
		{
			return operatorType switch
			{
				TokenType.Minus => CUnaryOperator.Negate,
				TokenType.Bang => CUnaryOperator.Bang,
				TokenType.RefMut => CUnaryOperator.AddressOf,
				TokenType.Ampersand => CUnaryOperator.AddressOf,
				TokenType.Star => CUnaryOperator.Dereference,
				_ => throw new ArgumentException("Invalid unary operator for transpiling " + operatorType)
			};
		}

		public static CLiteral ToLiteral(Literal literal)
		{
			return literal.Val.Type switch
			{
				TokenType.True => new CLiteral(true, CLiteralType.True),
				TokenType.False => new CLiteral(false, CLiteralType.False),
				TokenType.IntagerLiteral => new CLiteral(int.Parse(literal.Val.Text), CLiteralType.Intager),
				TokenType.FloatLiteral => new CLiteral(float.Parse(literal.Val.Text), CLiteralType.Float),
				TokenType.CharactorLiteral => new CLiteral(literal.Val.Text.Trim('\''), CLiteralType.Charactor), // always in format 'c' or '\n'
				TokenType.CStringLiteral => new CLiteral(literal.Val.Text.Trim('\"'), CLiteralType.String),
				_ => throw new ArgumentException("Invalid literal type for transpiling " + literal.Val.Type)
			};
		}
	}
}
