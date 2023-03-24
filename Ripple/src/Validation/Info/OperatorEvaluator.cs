using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.Utils.Extensions;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Types;

namespace Ripple.Validation.Info
{
    class OperatorEvaluator<TPrimary, TArg>
    {
        private readonly List<Func<TPrimary, TArg, LifetimeInfo, Option<ValueInfo>>> m_EvaluationFunctions = new ();
        private readonly Func<TPrimary, TArg, Token, ASTInfoError> m_TooManyOperatorsErrorGenerator;
        private readonly Func<TPrimary, TArg, Token, ASTInfoError> m_NoOperatorsErrorGenerator;

        public OperatorEvaluator(Func<TPrimary, TArg, Token, ASTInfoError> tooManyOperatorsErrorGenerator, Func<TPrimary, TArg, Token, ASTInfoError> noOperatorsErrorGenerator)
        {
            m_TooManyOperatorsErrorGenerator = tooManyOperatorsErrorGenerator;
            m_NoOperatorsErrorGenerator = noOperatorsErrorGenerator;
        }

        public Result<ValueInfo, ASTInfoError> Evaluate(TPrimary primary, TArg arg, LifetimeInfo currentLifetime, Token errorToken)
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
                return m_NoOperatorsErrorGenerator(primary, arg, errorToken);

            if (results.Count > 1)
                return m_TooManyOperatorsErrorGenerator(primary, arg, errorToken);

            return results[0];
        }

        public void AddOperatorEvaluator(Func<TPrimary, TArg, LifetimeInfo, Option<ValueInfo>> evaluator)
        {
            m_EvaluationFunctions.Add(evaluator);
        }
	}
}
