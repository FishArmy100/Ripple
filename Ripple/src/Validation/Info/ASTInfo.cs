using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.Lexing;
using Ripple.AST;

namespace Ripple.Validation.Info
{
    class ASTInfo
    {
        public readonly List<string> PrimaryTypes;
        public readonly List<ASTInfoError> Errors;
        public readonly FunctionList Functions;
        public readonly Dictionary<string, VariableInfo> GlobalVariables;
        public readonly OperatorEvaluatorLibrary OperatorLibrary;

        public readonly ProgramStmt AST;

        public ASTInfo(ProgramStmt ast, List<string> primaryTypes, FunctionList additionalFunctions, OperatorEvaluatorLibrary operatorLibrary)
        {
            PrimaryTypes = primaryTypes;
            OperatorLibrary = operatorLibrary;

            FunctionFinderHelper functionFinder = new FunctionFinderHelper(ast, PrimaryTypes, additionalFunctions);
            Errors = functionFinder.Errors;
            Functions = functionFinder.Functions;

            GlobalVariableFinderHelper globalVariableFinder = new GlobalVariableFinderHelper(ast, primaryTypes, Functions, operatorLibrary);
            GlobalVariables = globalVariableFinder.GlobalVariables;
            Errors.AddRange(globalVariableFinder.Errors);

            AST = ast;
        }
    }
}
