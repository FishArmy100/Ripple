using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.C_AST;

namespace Ripple.Transpiling.ASTConversion
{
    class ExpressionConversionResult
    {
        public readonly IReadOnlyList<CVarDecl> GeneratedVariables;
        public readonly CExpression Expression;
        public readonly CType ExpressionType;
        public readonly ExpressionValueType ValueType;

        public ExpressionConversionResult(IReadOnlyList<CVarDecl> generatedVariables, CExpression expression, CType expressionType, ExpressionValueType valueType)
        {
            GeneratedVariables = generatedVariables;
            Expression = expression;
            ExpressionType = expressionType;
            ValueType = valueType;
        }
    }
}
