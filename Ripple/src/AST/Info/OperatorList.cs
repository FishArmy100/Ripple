using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class OperatorList<TOperator, TPrimary, TArgs> where TOperator : class
    {
        private readonly Dictionary<TPrimary, List<TOperator>> m_Operators = new Dictionary<TPrimary, List<TOperator>>();
        private readonly Func<TOperator, TArgs, bool> m_Selector;
        private readonly Func<TOperator, TPrimary> m_PrimaryGetter;
        private readonly Func<TOperator, TArgs> m_ArgsGetter;
        private readonly Func<TPrimary, TArgs, Option<TOperator>> m_IntrinsicOperatorGetter;

        public OperatorList(Func<TOperator, TArgs, bool> selector, Func<TOperator, TPrimary> primaryGetter, Func<TOperator, TArgs> argsGetter, Func<TPrimary, TArgs, Option<TOperator>> intrinsicOperatorGetter)
        {
            m_Selector = selector;
            m_PrimaryGetter = primaryGetter;
            m_ArgsGetter = argsGetter;
            m_IntrinsicOperatorGetter = intrinsicOperatorGetter;
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

        public bool TryGet(TPrimary primary, TArgs args, out TOperator operatorType)
        {
            Option<TOperator> op = m_IntrinsicOperatorGetter(primary, args);
            if(op.HasValue())
            {
                operatorType = op.Value;
                return true;
            }

            List<TOperator> overloads = m_Operators.GetOrCreate(primary);
            operatorType = overloads.FirstOrDefault(o => m_Selector(o, args));
            return operatorType != null;
        }

        public bool Contains(TPrimary primary, TArgs args) => TryGet(primary, args, out _);
    }
}
