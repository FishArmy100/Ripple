using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Utils.Extensions;
using Ripple.Lexing;

namespace Ripple.Validation
{
    static class Validator
    {
        public static List<ValidationError> ValidateAst(FileStmt fileStmt)
        {
            AstDeclarationData data = GetBuiltinDeclarations();
            AstDeclarationFinder.AppendData(fileStmt, ref data);
            (_, var errors) = CreateHelper(data);
            return errors;
        }

        private static (TypeCheckerHelper, List<ValidationError>) CreateHelper(AstDeclarationData data)
        {
            List<ValidationError> errors = new List<ValidationError>();

            Dictionary<string, VariableData> globalVariables = new Dictionary<string, VariableData>();
            foreach (var variable in data.GlobalVariables)
            {
                if (!globalVariables.TryAdd(variable.Name.Text, variable))
                {
                    string errorMessage = "Global variable: " + variable.Name.Text + " has already been defined";
                    errors.Add(new ValidationError(errorMessage, variable.Name));
                }
            }

            Dictionary<string, List<FunctionData>> globalFunctions = new Dictionary<string, List<FunctionData>>();
            foreach(FunctionData function in data.GlobalFunctions)
            {
                string funcName = function.Name.Text;
                if(globalFunctions.TryGetValue(funcName, out var functionOverloads))
                {
                    bool hasDuplicate = functionOverloads
                        .Any(fn => fn.Parameters.SequenceEquals(function.Parameters, (a, b) =>
                        {
                            return a.Item1.Text == b.Item1.Text;
                        }));

                    if(hasDuplicate)
                    {
                        string message = "Global function: " + funcName + " has already been defined";
                        errors.Add(new ValidationError(message, function.Name));
                    }
                    else
                    {
                        globalFunctions[funcName].Add(function);
                    }
                }
                else
                {
                    List<FunctionData> newOverloads = new List<FunctionData>() { function };
                    globalFunctions.Add(funcName, newOverloads);
                }
            }

            Dictionary<string, TypeData> types = data.Types.ToDictionary(t => t.Name);

            Dictionary<TokenType, List<OperatorData>> operators =
                data.Operators
                .GroupBy(op => op.OperatorType)
                .ToDictionary(g => g.Key, g => g.ToList());

            TypeCheckerHelper helper = new TypeCheckerHelper(globalVariables, operators, globalFunctions, types);
            return (helper, errors);
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
