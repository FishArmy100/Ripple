using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class OperatorList
    {
        Dictionary<TokenType, List<OperatorData>> m_Operators = new Dictionary<TokenType, List<OperatorData>>();

        public bool TryAddOperator(OperatorData operatorData)
        {
            if (ContainsOperator(operatorData))
                return false;

            if (m_Operators.TryGetValue(operatorData.OperatorType, out List<OperatorData> overloads))
            {
                overloads.Add(operatorData);
            }
            else
            {
                List<OperatorData> newOverloads = new() { operatorData };
                m_Operators.Add(operatorData.OperatorType, newOverloads);
            }

            return true;
        }

        public bool TryGetOperator<TOp>(TokenType name, List<string> parameterTypes, out TOp operatorData) where TOp : OperatorData
        {
            if(m_Operators.TryGetValue(name, out List<OperatorData> overloads))
            {
                operatorData = overloads
                    .FirstOrDefault(o => o.IsOperator(name, parameterTypes))
                    as TOp;

                return operatorData != null;
            }

            operatorData = null;
            return false;
        }

        public bool ContainsOperator<TOp>(TokenType name, List<string> parameterTypes) where TOp : OperatorData
        {
            return TryGetOperator<TOp>(name, parameterTypes, out _);
        }

        public bool ContainsOperator(OperatorData operatorData)
        {
            if (m_Operators.TryGetValue(operatorData.OperatorType, out var operatorOverloads))
            {
                return operatorOverloads.Any(o => o.IsOperator(operatorData));
            }

            return false;
        }
    }
}
