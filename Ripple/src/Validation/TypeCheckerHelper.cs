using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Validation.AstInfo;

namespace Ripple.Validation
{
    class TypeCheckerHelper
    {
        private readonly Dictionary<string, VariableData> m_GlobalVariables;
        private readonly OperatorList m_Operators;
        private readonly FunctionList m_GlobalFuncitons;
        private readonly Dictionary<string, TypeData> m_Types;

        public TypeCheckerHelper(Dictionary<string, VariableData> globalVariables, 
                                 OperatorList operators,
                                 FunctionList globalFuncitons, 
                                 Dictionary<string, TypeData> types)
        {
            m_GlobalVariables = globalVariables;
            m_Operators = operators;
            m_GlobalFuncitons = globalFuncitons;
            m_Types = types;
        }

        public bool TryGetVariable(string name, out VariableData variable)
        {
            return m_GlobalVariables.TryGetValue(name, out variable);
        }

        public bool ContainsVariable(string name)
        {
            return m_GlobalVariables.ContainsKey(name);
        }

        public bool TryGetOperator<TOp>(TokenType operatorType, List<string> paramTypes, out TOp operatorData) where TOp : OperatorData
        {
            return m_Operators.TryGetOperator<TOp>(operatorType, paramTypes, out operatorData);
        }

        public bool ContainsOperator<TOp>(TokenType operatorType, List<string> paramTypes) where TOp : OperatorData
        {
            return m_Operators.ContainsOperator<TOp>(operatorType, paramTypes);
        }

        public bool TryGetFunction(string name, List<string> paramTypes, out FunctionData functionData)
        {
            return m_GlobalFuncitons.TryGetFunction(name, paramTypes, out functionData);
        }

        public bool ContainsFunction(string name, List<string> paramTypes)
        {
            return m_GlobalFuncitons.ContainsFunction(name, paramTypes);
        }

        public bool ContainsFunctionWithName(string name)
        {
            return m_GlobalFuncitons.ContainsFunctionWithName(name);
        }

        public bool TryGetType(string name, out TypeData typeData)
        {
            return m_Types.TryGetValue(name, out typeData);
        }

        public bool ContainsType(string name)
        {
            return m_Types.ContainsKey(name);
        }
    }
}
