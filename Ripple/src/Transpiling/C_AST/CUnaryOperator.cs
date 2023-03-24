using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling.C_AST
{
	enum CUnaryOperator
	{
		Negate,
		Bang,
		AddressOf,
		Dereference,
	}

	static class CUnaryOperatorExtensions
	{
		public static string ToCCode(this CUnaryOperator self)
		{
			return self switch
			{
				CUnaryOperator.Negate => "-",
				CUnaryOperator.Bang => "!",
				CUnaryOperator.AddressOf => "&",
				CUnaryOperator.Dereference => "*",
				_ => throw new ArgumentException("Enum type: " + self + " not supported."),
			};

		}
	}
}
