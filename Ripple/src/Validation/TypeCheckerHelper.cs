using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;

namespace Ripple.Validation
{
    class TypeCheckerHelper
    {
        private Dictionary<string, VariableData> m_GlobalVariables;
        private Dictionary<TokenType, List<OperatorData>> m_Operators;
        private Dictionary<string, List<FunctionData>> m_GlobalFuncitons;
        private Dictionary<string, TypeData> m_Types;

        public TypeCheckerHelper(Dictionary<string, VariableData> globalVariables, 
                                   Dictionary<TokenType, List<OperatorData>> operators, 
                                   Dictionary<string, List<FunctionData>> globalFuncitons, 
                                   Dictionary<string, TypeData> types)
        {
            m_GlobalVariables = globalVariables;
            m_Operators = operators;
            m_GlobalFuncitons = globalFuncitons;
            m_Types = types;
        }

        ValidationError? TryAddDeclaration(VariableData globalVariableData)
        {
            if (!m_GlobalVariables.TryAdd(globalVariableData.Name.Text, globalVariableData))
                return new ValidationError("Global variable: " + globalVariableData.Name + " is already defined.", globalVariableData.Name);

            return null;
        }

        public bool TryGetVariable(string name, out VariableData globalVariableData)
        {
            return m_GlobalVariables.TryGetValue(name, out globalVariableData);
        }
    }
}
