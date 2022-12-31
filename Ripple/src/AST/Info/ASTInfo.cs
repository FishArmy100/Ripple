using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.AST.Utils;
using Ripple.AST.Info;
using Ripple.Lexing;

namespace Ripple.AST.Info
{
    class ASTInfo
    {
        public readonly List<PrimaryTypeInfo> PrimaryTypes;
        public readonly List<ASTInfoError> Errors;
        public readonly FunctionList Functions;
        public readonly Dictionary<string, VariableInfo> GlobalVariables;
        public readonly OperatorEvaluatorLibrary OperatorLibrary;

        public readonly ProgramStmt AST;

        public ASTInfo(ProgramStmt ast, List<PrimaryTypeInfo> primaryTypes, FunctionList additionalFunctions, OperatorEvaluatorLibrary operatorLibrary)
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

        private class GlobalVariableFinderHelper : ASTWalkerBase
        {
            private readonly List<PrimaryTypeInfo> m_Primaries;
            private readonly FunctionList m_Functions;
            private readonly List<string> m_FunctionNames;
            private readonly OperatorEvaluatorLibrary m_Operators;

            public readonly List<ASTInfoError> Errors = new List<ASTInfoError>();
            public readonly Dictionary<string, VariableInfo> GlobalVariables = new Dictionary<string, VariableInfo>();

            public GlobalVariableFinderHelper(ProgramStmt ast, List<PrimaryTypeInfo> primaries, FunctionList functions, OperatorEvaluatorLibrary operators)
            {
                m_Primaries = primaries;
                m_Functions = functions;
                m_FunctionNames = functions.GetFunctionNames();
                m_Operators = operators;
                ast.Accept(this);
            }

            public override void VisitVarDecl(VarDecl varDecl)
            {
                LocalVariableStack variableStack = new LocalVariableStack();
                ValueOfExpressionVisitor visitor = new ValueOfExpressionVisitor(variableStack, m_Functions, m_Operators, new Dictionary<string, VariableInfo>());
                var result = VariableInfo.FromVarDecl(varDecl, visitor, m_Primaries, new List<Token>(), LifetimeInfo.Static, varDecl.UnsafeToken.HasValue);
                result.Match(ok =>
                {
                    if(CheckType(ok[0].Type))
                    {
                        foreach (VariableInfo info in ok)
                            TryAddVariableInfo(info);
                    }
                },
                fail =>
                {
                    Errors.AddRange(fail);
                });
            }

            private void TryAddVariableInfo(VariableInfo variableInfo)
            {
                string varName = variableInfo.Name;
                if(m_FunctionNames.Contains(varName))
                {
                    AddError("Variable name '" + varName + "' is already used as a function name.", variableInfo.NameToken);
                    return;
                }

                if(GlobalVariables.ContainsKey(varName))
                {
                    AddError("Global variable with name '" + varName + "' already exists.", variableInfo.NameToken);
                    return;
                }

                GlobalVariables.Add(varName, variableInfo);
            }

            private bool CheckType(TypeInfo info)
            {
                List<PrimaryTypeInfo> primaries = info.GetPrimaries();

                bool isValid = true;
                foreach (PrimaryTypeInfo primary in primaries)
                {
                    if (!m_Primaries.Contains(primary))
                    {
                        Errors.Add(new ASTInfoError("Undefined type: " + primary.Name.Text, primary.Name));
                        isValid = false;
                    }
                }

                if (info.HasNonPointerVoid())
                {
                    AddError("'void' type can only be used as a return type, otherwise, must be a pointer.", primaries[0].Name);
                    isValid = false;
                }

                return isValid;
            }

            private void AddError(string name, Token token)
            {
                Errors.Add(new ASTInfoError(name, token));
            }
        }

        private class FunctionFinderHelper : ASTWalkerBase
        {
            private readonly List<PrimaryTypeInfo> m_Primaries;
            private readonly List<string> m_GlobalVariableNames = new List<string>();
            
            public List<ASTInfoError> Errors { get; private set; } = new List<ASTInfoError>();
            public FunctionList Functions { get; private set; }

            public FunctionFinderHelper(ProgramStmt ast, List<PrimaryTypeInfo> primaries, FunctionList additionalFunctions)
            {
                Functions = additionalFunctions;
                m_Primaries = primaries;
                ast.Accept(this);
            }

            private Option<TypeInfo> CheckType(TypeInfo info, bool isReturnType)
            {
                List<PrimaryTypeInfo> primaries = info.GetPrimaries();

                foreach(PrimaryTypeInfo primary in primaries)
                {
                    if(!m_Primaries.Contains(primary))
                    {
                        Errors.Add(new ASTInfoError("Undefined type: " + primary.Name.Text, primary.Name));
                    }
                }

                if(!isReturnType && info.HasNonPointerVoid())
                {
                    AddError("'void' type can only be used as a return type, otherwise, must be a pointer.", primaries[0].Name);
                }

                return new Option<TypeInfo>();
            }

            public override void VisitFuncDecl(FuncDecl funcDecl)
            {
                FunctionInfo.FromASTFunction(funcDecl, m_Primaries).Match(ok =>
                {
                    CheckFunctionInfo(ok);
                },
                fail =>
                {
                    Errors.AddRange(fail);
                });
            }

            public override void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
            {
                FunctionInfo.FromASTExternalFunction(externalFuncDecl, m_Primaries).Match(ok =>
                {
                    CheckFunctionInfo(ok);
                },
                fail =>
                {
                    Errors.AddRange(fail);
                });
            }

            private void CheckFunctionInfo(FunctionInfo info)
            {
                CheckType(info.ReturnType, true);
                foreach (TypeInfo paramType in info.Parameters.ConvertAll(p => p.Type))
                    CheckType(paramType, false);

                if (m_GlobalVariableNames.Contains(info.Name))
                {
                    AddError("Variable with the name: " + info.Name + ", has already been defined.", info.NameToken);
                }
                else if (!Functions.TryAddFunction(info))
                {
                    Errors.Add(new ASTInfoError("Function " + info.Name + ", has already been defined.", info.NameToken));
                }
            }

            private void AddError(string message, Token token)
            {
                Errors.Add(new ASTInfoError(message, token));
            }
        }
    }
}
