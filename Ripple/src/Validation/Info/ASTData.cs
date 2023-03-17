using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Validation.Info
{
	class ASTData
	{
        public readonly IReadOnlyList<string> PrimaryTypes;
        public readonly FunctionList Functions;
        public readonly IReadOnlyDictionary<string, VariableInfo> GlobalVariables;
        public readonly OperatorEvaluatorLibrary OperatorLibrary;

		public ASTData(IReadOnlyList<string> primaryTypes, FunctionList functions, IReadOnlyDictionary<string, VariableInfo> globalVariables, OperatorEvaluatorLibrary operatorLibrary)
		{
			PrimaryTypes = primaryTypes;
			Functions = functions;
			GlobalVariables = globalVariables;
			OperatorLibrary = operatorLibrary;
		}
	}
}
