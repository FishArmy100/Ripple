using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;
using Ripple.Utils;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class OperatorList<TOperator, TPrimary, TArgs> where TOperator : class
    {
        private readonly Dictionary<TPrimary, List<TOperator>> m_Operators = new Dictionary<TPrimary, List<TOperator>>();
        private readonly Func<TOperator, TArgs, bool> m_Selector;
        private readonly Func<TOperator, TPrimary> m_PrimaryGetter;
        private readonly Func<TOperator, TArgs> m_ArgsGetter;
        private readonly Func<TPrimary, TArgs, Option<Result<TOperator, ASTInfoError>>> m_IntrinsicOperatorGetter;
        private readonly Func<TPrimary, TArgs, Token, ASTInfoError> m_GenErrorFunc;

        public OperatorList(Func<TOperator, TArgs, bool> selector, 
                            Func<TOperator, TPrimary> primaryGetter, 
                            Func<TOperator, TArgs> argsGetter, 
                            Func<TPrimary, TArgs, Option<Result<TOperator, ASTInfoError>>> intrinsicOperatorGetter, 
                            Func<TPrimary, TArgs, Token, ASTInfoError> genErrorFunc)
        {
            m_Selector = selector;
            m_PrimaryGetter = primaryGetter;
            m_ArgsGetter = argsGetter;
            m_IntrinsicOperatorGetter = intrinsicOperatorGetter;
            m_GenErrorFunc = genErrorFunc;
        }

        public bool TryAdd(TOperator op)
        {
            if (m_IntrinsicOperatorGetter(m_PrimaryGetter(op), m_ArgsGetter(op)).HasValue())
                return false;

            List<TOperator> overloads = m_Operators.GetOrCreate(m_PrimaryGetter(op));
            if (!overloads.Any(o => m_Selector(o, m_ArgsGetter(op))))
            {
                overloads.Add(op);
                return true;
            }

            return false;
        }

        public Result<TOperator, ASTInfoError> TryGet(TPrimary primary, TArgs args, Token errorToken)
        {
            var op = m_IntrinsicOperatorGetter(primary, args);
            return op.Match(result =>
            {
                return result;
            },
            () => 
            {
                List<TOperator> overloads = m_Operators.GetOrCreate(primary);
                var operatorData = overloads.FirstOrDefault(o => m_Selector(o, args));
                if (operatorData != null)
                    return operatorData;

                return new Result<TOperator, ASTInfoError>(m_GenErrorFunc(primary, args, errorToken));
            });
        }

        public bool Contains(TPrimary primary, TArgs args) => 
            TryGet(primary, args, new Token()).IsOk();
    }
}
