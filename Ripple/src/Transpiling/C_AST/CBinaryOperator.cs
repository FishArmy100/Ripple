using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling.C_AST
{
	public enum CBinaryOperator
	{
		Plus,
		Minus,
		Star,
		Divide,
		Assign,
		Mod,
		EqualEqual,
		BangEqual,
		GreaterThan,
		LessThan,
		GreaterThanEqual,
		LessThanEqual,
		AnpersandAnpersand,
		PipePipe,
	}

	public static class CBinaryOperatorExtensions
	{
		public static string ToCCode(this CBinaryOperator self)
		{
			return self switch
			{
				CBinaryOperator.Plus => "+",
				CBinaryOperator.Minus => "-",
				CBinaryOperator.Star => "*",
				CBinaryOperator.Divide => "/",
				CBinaryOperator.Assign => "=",
				CBinaryOperator.Mod => "%",
				CBinaryOperator.EqualEqual => "==",
				CBinaryOperator.BangEqual => "!=",
				CBinaryOperator.GreaterThan => ">",
				CBinaryOperator.LessThan => "<",
				CBinaryOperator.GreaterThanEqual => ">=",
				CBinaryOperator.LessThanEqual => "<=",
				CBinaryOperator.AnpersandAnpersand => "&&",
				CBinaryOperator.PipePipe => "||",
				_ => throw new ArgumentException("Enum type: " + self + " not supported."),
			};
		}
	}
}
