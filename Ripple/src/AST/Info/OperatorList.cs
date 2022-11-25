using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class OperatorList
    {
        Dictionary<TokenType, List<OperatorInfo>> m_Operators = new Dictionary<TokenType, List<OperatorInfo>>();

        public bool TryAddOperator(OperatorInfo operatorData)
        {
            if (ContainsOperator(operatorData))
                return false;

            if (m_Operators.TryGetValue(operatorData.OperatorType, out List<OperatorInfo> overloads))
            {
                overloads.Add(operatorData);
            }
            else
            {
                List<OperatorInfo> newOverloads = new() { operatorData };
                m_Operators.Add(operatorData.OperatorType, newOverloads);
            }

            return true;
        }

        public bool TryGetOperator<TOp>(TokenType name, List<string> parameterTypes, out TOp operatorData) where TOp : OperatorInfo
        {
            if (m_Operators.TryGetValue(name, out List<OperatorInfo> overloads))
            {
                operatorData = overloads
                    .FirstOrDefault(o => o.IsOperator(name, parameterTypes))
                    as TOp;

                return operatorData != null;
            }

            operatorData = null;
            return false;
        }

        public bool ContainsOperator<TOp>(TokenType name, List<string> parameterTypes) where TOp : OperatorInfo
        {
            return TryGetOperator<TOp>(name, parameterTypes, out _);
        }

        public bool ContainsOperator(OperatorInfo operatorData)
        {
            if (m_Operators.TryGetValue(operatorData.OperatorType, out var operatorOverloads))
            {
                return operatorOverloads.Any(o => o.IsOperator(operatorData));
            }

            return false;
        }
    }
}
