using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;
using Ripple.AST;
using Ripple.Utils;
using Ripple.Transpiling.SourceGeneration;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info.Expressions;

namespace Ripple.Transpiling.ASTConversion
{
	static class ASTConverter
	{
		public static CProgram ConvertAST(ProgramStmt program)
		{
			return null;
		}

		public static CType ConvertToCType(TypeInfo name, ref CArrayRegistry registry)
		{
			return name.Accept(new TypeConverterVisitor(registry));
		}

		public static ExpressionConversionResult ConvertExpression(TypedExpression expression, ref CArrayRegistry registry, string tempVarPostfix)
        {
			return expression.Accept(new ExpressionConverterVisitor(registry, tempVarPostfix));
        }
	}
}
