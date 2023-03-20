using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.Lexing;
using Ripple.AST;
using Ripple.Validation.Info.Expressions;

namespace Ripple.Transpiling.ASTConversion
{
	static class ASTConversionUtils
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

		public static CLiteral ToLiteral(TypedLiteral literal)
		{
			return literal.Type switch
			{
				TokenType.True => new CLiteral(true, CLiteralType.True),
				TokenType.False => new CLiteral(false, CLiteralType.False),
				TokenType.IntagerLiteral => new CLiteral(int.Parse(literal.Value), CLiteralType.Intager),
				TokenType.FloatLiteral => new CLiteral(float.Parse(literal.Value), CLiteralType.Float),
				TokenType.CharactorLiteral => new CLiteral(literal.Value.Trim('\''), CLiteralType.Charactor), // always in format 'c' or '\n'
				TokenType.CStringLiteral => new CLiteral(literal.Value.Trim('\"'), CLiteralType.String),
				TokenType.Nullptr => new CLiteral("nullptr", CLiteralType.Nullptr),
				_ => throw new ArgumentException("Invalid literal type for transpiling " + literal.Type)
			};
		}
	}
}
