using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.AST;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info.Expressions;
using Raucse;
using Ripple.Validation.Errors;
using Ripple.Validation.Info.Values;

namespace Ripple.Validation.Info.Checking
{
    static class ExpressionChecker
    {

        public static Result<Pair<ValueInfo, TypedExpression>, List<ValidationError>> CheckExpression(Expression expression, ASTData ast, LocalVariableStack variableStack, SafetyContext safetyContext, List<string> activeLifetimes, Option<TypeInfo> expected = default)
        {
            ExpressionCheckerVisitor visitor = new ExpressionCheckerVisitor(ast, variableStack, activeLifetimes, safetyContext);
            return CheckExpression(expression, visitor, expected);
        }

        public static Result<Pair<ValueInfo, TypedExpression>, List<ValidationError>> CheckExpression(Expression expression, ExpressionCheckerVisitor visitor, Option<TypeInfo> expected = default)
        {
            try
            {
                Option<ExpectedValue> expectedVal = expected.MatchOrConstruct(e => new Option<ExpectedValue>(new ExpectedValue(e, false)));
                return expression.Accept(visitor, expectedVal);
            }
            catch (ExpressionCheckerException e)
            {
                return e.Errors.ToList();
            }

        }
    }
}
