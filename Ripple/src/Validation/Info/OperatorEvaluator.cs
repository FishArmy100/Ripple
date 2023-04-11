using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Types;
using Raucse;
using Ripple.Validation.Errors;
using Ripple.Core;

namespace Ripple.Validation.Info
{
    public class OperatorEvaluator<TPrimary, TArg>
    {
        private readonly List<Func<TPrimary, TArg, LifetimeInfo, Option<ValueInfo>>> m_EvaluationFunctions = new ();
        private readonly Func<TPrimary, TArg, SourceLocation, ValidationError> m_TooManyOperatorsErrorGenerator;
        private readonly Func<TPrimary, TArg, SourceLocation, ValidationError> m_NoOperatorsErrorGenerator;

        public OperatorEvaluator(Func<TPrimary, TArg, SourceLocation, ValidationError> tooManyOperatorsErrorGenerator, Func<TPrimary, TArg, SourceLocation, ValidationError> noOperatorsErrorGenerator)
        {
            m_TooManyOperatorsErrorGenerator = tooManyOperatorsErrorGenerator;
            m_NoOperatorsErrorGenerator = noOperatorsErrorGenerator;
        }

        public Result<ValueInfo, ValidationError> Evaluate(TPrimary primary, TArg arg, LifetimeInfo currentLifetime, SourceLocation errorLocation)
        {
            List<ValueInfo> results = new List<ValueInfo>();
            foreach(var evaluator in m_EvaluationFunctions)
            {
                evaluator(primary, arg, currentLifetime).Match(ok =>
                {
                    results.Add(ok);
                });
            }

            if (results.Count == 0)
                return m_NoOperatorsErrorGenerator(primary, arg, errorLocation);

            if (results.Count > 1)
                return m_TooManyOperatorsErrorGenerator(primary, arg, errorLocation);

            return results[0];
        }

        public void AddOperatorEvaluator(Func<TPrimary, TArg, LifetimeInfo, Option<ValueInfo>> evaluator)
        {
            m_EvaluationFunctions.Add(evaluator);
        }
	}
}
