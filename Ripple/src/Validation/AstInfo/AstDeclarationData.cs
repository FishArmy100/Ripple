using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.AstInfo
{
    class AstDeclarationData
    {
        public readonly List<TypeData> Types;
        public readonly List<FunctionData> GlobalFunctions;
        public readonly List<OperatorData> Operators;
        public readonly List<VariableData> GlobalVariables;

        public AstDeclarationData(List<TypeData> types, List<FunctionData> globalFunctions, List<OperatorData> operators, List<VariableData> globalVariables)
        {
            Types = types;
            GlobalFunctions = globalFunctions;
            Operators = operators;
            GlobalVariables = globalVariables;
        }
    }
}
