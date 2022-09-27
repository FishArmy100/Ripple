using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Utils.Extensions;
using Ripple.Lexing;
using Ripple.Utils;

namespace Ripple.Validation
{
    static class Validator
    {
        public static Result<FileStmt, List<ValidationError>> ValidateAst(FileStmt fileStmt)
        {
            AstDeclarationData data = GetBuiltinDeclarations();
            AstDeclarationFinder.AppendDeclarations(fileStmt, ref data);

            List<ValidationError> errors = new List<ValidationError>();
            TypeCheckerHelper helper = CreateHelper(data, ref errors);
            TypeCheckerVisitor typeChecker = new TypeCheckerVisitor(helper);
            errors.AddRange(typeChecker.TypeCheck(fileStmt));

            if (errors.Count > 0)
                return new Result<FileStmt, List<ValidationError>>.Fail(errors);

            return new Result<FileStmt, List<ValidationError>>.Ok(fileStmt);
        }

        private static TypeCheckerHelper CreateHelper(AstDeclarationData data, ref List<ValidationError> errors)
        {
            Dictionary<string, TypeData> types = data.Types.ToDictionary(t => t.Name);
            FunctionList globalFunctions = GetFunctionList(data, ref errors, types);
            Dictionary<string, VariableData> globalVariables = GetGlobalVariables(data, ref errors, types, globalFunctions);
            OperatorList operators = GetOperators(data, ref errors);

            TypeCheckerHelper helper = new TypeCheckerHelper(globalVariables, operators, globalFunctions, types);
            return helper;
        }

        private static OperatorList GetOperators(AstDeclarationData data, ref List<ValidationError> errors)
        {
            OperatorList operators = new OperatorList();
            foreach (OperatorData operatorData in data.Operators)
            {
                if (!operators.TryAddOperator(operatorData))
                {
                    string message = "Operator " + operatorData.OperatorType + " is already defined.";
                    errors.Add(new ValidationError(message, new Token()));
                }
            }

            return operators;
        }

        private static Dictionary<string, VariableData> GetGlobalVariables(AstDeclarationData data, ref List<ValidationError> errors, Dictionary<string, TypeData> types, FunctionList globalFunctions)
        {
            Dictionary<string, VariableData> globalVariables = new Dictionary<string, VariableData>();
            foreach (var variable in data.GlobalVariables)
            {
                if (types.ContainsKey(variable.Name.Text))
                {
                    string message = "Cannot name global variable: " + variable.Name.Text + " the name of a type";
                    errors.Add(new ValidationError(message, variable.Name));
                    continue;
                }

                if (globalFunctions.ContainsFunctionWithName(variable.Name.Text))
                {
                    string message = "Cannot name global variable: " + variable.Name.Text + " the name of a function";
                    errors.Add(new ValidationError(message, variable.Name));
                    continue;
                }

                if (!globalVariables.TryAdd(variable.Name.Text, variable))
                {
                    string errorMessage = "Global variable: " + variable.Name.Text + " has already been defined";
                    errors.Add(new ValidationError(errorMessage, variable.Name));
                }
            }

            return globalVariables;
        }

        private static FunctionList GetFunctionList(AstDeclarationData data, ref List<ValidationError> errors, Dictionary<string, TypeData> types)
        {
            FunctionList globalFunctions = new FunctionList();
            foreach (FunctionData function in data.GlobalFunctions)
            {
                if (types.ContainsKey(function.Name.Text))
                {
                    string message = "Cannot name global function: " + function.Name.Text + " the name of a type";
                    errors.Add(new ValidationError(message, function.Name));
                    continue;
                }

                if (!globalFunctions.TryAddFunction(function))
                {
                    string funcName = function.Name.Text;
                    string message = "Global function: " + funcName + " is already defined.";
                    errors.Add(new ValidationError(message, function.Name));
                }
            }

            return globalFunctions;
        }

        private static AstDeclarationData GetBuiltinDeclarations()
        {
            List<TypeData> types = RippleBuiltins.GetPrimitives();
            List<FunctionData> globalFunctions = RippleBuiltins.GetBuiltInFunctions();
            List<OperatorData> operators = RippleBuiltins.GetPrimitiveOperators();
            return new AstDeclarationData(types, globalFunctions, operators, new List<VariableData>());
        }
    }
}
