using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils.Extensions;

namespace Ripple.AST.Info
{
    class OperatorLibrary
    {
        public readonly OperatorList<OperatorInfo.Unary, TokenType, TypeInfo> UnaryOperators =
            new((u, type) => u.Operand == type, u => u.OperatorType, u => u.Operand);

        public readonly OperatorList<OperatorInfo.Binary, TokenType, (TypeInfo, TypeInfo)> BinaryOperators =
            new((u, args) => u.Left == args.Item1 && u.Right == args.Item2, u => u.OperatorType, u => (u.Left, u.Right));

        public readonly OperatorList<OperatorInfo.Cast, TypeInfo, TypeInfo> CastOperators =
            new((u, type) => u.Operand == type, u => u.TypeToCastTo, u => u.Operand);

        public readonly OperatorList<OperatorInfo.Call, TypeInfo, List<TypeInfo>> CallOperators =
            new((u, args) => u.Arguments.SequenceEqual(args), u => u.Callee, u => u.Arguments);

        public readonly OperatorList<OperatorInfo.Index, TypeInfo, TypeInfo> IndexOperators =
            new((u, arg) => u.Argument == arg, u => u.Indexed, u => u.Argument);

        public class OperatorList<TOperator, TPrimary, TArgs>
        {
            private readonly Dictionary<TPrimary, List<TOperator>> m_Operators = new Dictionary<TPrimary, List<TOperator>>();
            private readonly Func<TOperator, TArgs, bool> m_Selector;
            private readonly Func<TOperator, TPrimary> m_PrimaryGetter;
            private readonly Func<TOperator, TArgs> m_ArgsGetter;

            public OperatorList(Func<TOperator, TArgs, bool> selector, Func<TOperator, TPrimary> primaryGetter, Func<TOperator, TArgs> argsGetter)
            {
                m_Selector = selector;
                m_PrimaryGetter = primaryGetter;
                m_ArgsGetter = argsGetter;
            }

            public bool TryAdd(TOperator op)
            {
                List<TOperator> overloads = m_Operators.GetOrCreate(m_PrimaryGetter(op));
                if (!overloads.Any(o => m_Selector(o, m_ArgsGetter(op))))
                {
                    overloads.Add(op);
                    return true;
                }

                return false;
            }

            public bool TryGet(TPrimary primary, TArgs args, out TOperator unary)
            {
                List<TOperator> overloads = m_Operators.GetOrCreate(primary);
                unary = overloads.FirstOrDefault(o => m_Selector(o, args));
                return unary != null;
            }

            public bool Contains(TPrimary primary, TArgs args) => TryGet(primary, args, out _);
        }

        
    }
}
