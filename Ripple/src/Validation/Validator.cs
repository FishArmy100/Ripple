using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;
using Ripple.Parsing;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Statements;
using Raucse;
using Ripple.Validation.Errors;
using Ripple.Validation.Info.Checking;
using Ripple.Validation.Info.Functions;

namespace Ripple.Validation
{
    public static class Validator
    {
        public static Result<TypedProgramStmt, List<ValidationError>> ValidateAst(ProgramStmt programStmt)
        {
            var info = GenerateASTData(programStmt);
            StatementChecker checker = new StatementChecker(info.First, RippleBuiltins.GetBuiltInLinker());
            var result = programStmt.Accept(checker);

            List<ValidationError> errors = new List<ValidationError>();
            errors.AddRange(info.Second);
            result.Match(ok => { }, fail => errors.AddRange(fail));
            if (errors.Any())
                return errors;

            return (TypedProgramStmt)result.Value;
        }

        private static Pair<ASTData, List<ValidationError>> GenerateASTData(ProgramStmt programStmt)
        {
            List<string> primaries = RippleBuiltins.GetPrimitives();
            FunctionList functions = RippleBuiltins.GetBuiltInFunctions();
            OperatorEvaluatorLibrary library = RippleBuiltins.GetBuiltInOperators();

            FunctionFinderHelper functionFinder = new FunctionFinderHelper(programStmt, primaries, functions);
            GlobalVariableFinderHelper globalVariableFinder = new GlobalVariableFinderHelper(programStmt, primaries, functions, library);
            List<ValidationError> errors = new List<ValidationError>();

            functions = functionFinder.Functions;
            errors.AddRange(functionFinder.Errors);

            Dictionary<string, VariableInfo> globalVariables = globalVariableFinder.GlobalVariables;
            errors.AddRange(globalVariableFinder.Errors);

            ASTData data = new ASTData(primaries, functions, globalVariables, library);
            return new Pair<ASTData, List<ValidationError>>(data, errors);
        }
    }
}
