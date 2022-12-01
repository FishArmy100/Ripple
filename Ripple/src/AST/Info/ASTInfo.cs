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
        public readonly List<TypeInfo> CompositTypes;
        public readonly List<ASTInfoError> Errors;
        public readonly FunctionList Functions;
        public readonly Dictionary<string, VariableInfo> GlobalVariables;
        public readonly OperatorLibrary OperatorLibrary;

        public readonly ProgramStmt AST;

        public ASTInfo(ProgramStmt ast, List<PrimaryTypeInfo> primaryTypes, FunctionList additionalFunctions, OperatorLibrary operatorLibrary)
        {
            PrimaryTypes = primaryTypes;

            ASTInfoGenerationHelper helper = new ASTInfoGenerationHelper(ast, PrimaryTypes, additionalFunctions);
            CompositTypes = helper.CompositTypes;
            Errors = helper.Errors;
            Functions = helper.Functions;
            GlobalVariables = helper.GlobalVariables;

            OperatorLibrary = operatorLibrary;

            AST = ast;
        }

        private class ASTInfoGenerationHelper : AstWalkerBase
        {
            private readonly List<PrimaryTypeInfo> m_Primaries;
            public List<TypeInfo> CompositTypes { get; private set; } = new List<TypeInfo>();
            public List<ASTInfoError> Errors { get; private set; } = new List<ASTInfoError>();
            public FunctionList Functions { get; private set; }
            public Dictionary<string, VariableInfo> GlobalVariables { get; private set; } = new Dictionary<string, VariableInfo>();

            private bool m_IsInGlobalScope = true;

            public ASTInfoGenerationHelper(ProgramStmt ast, List<PrimaryTypeInfo> primaries, FunctionList additionalFunctions)
            {
                Functions = additionalFunctions;
                m_Primaries = primaries;
                ast.Accept(this);
            }

            private Option<TypeInfo> CheckType(TypeName typeName)
            {
                TypeInfo info = TypeInfo.FromASTType(typeName);
                List<PrimaryTypeInfo> primaries = info.GetPrimaries();

                bool isValidType = true;
                foreach(PrimaryTypeInfo primary in primaries)
                {
                    if(!m_Primaries.Contains(primary))
                    {
                        isValidType = false;
                        Errors.Add(new ASTInfoError("Undefined type: " + primary.Name.Text, primary.Name));
                    }
                }

                if (isValidType && !CompositTypes.Contains(info))
                {
                    CompositTypes.Add(info);
                    return info;
                }

                return new Option<TypeInfo>();
            }

            public override void VisitFuncDecl(FuncDecl funcDecl)
            {
                CheckType(funcDecl.ReturnType);
                foreach (TypeName typeName in funcDecl.Param.ParamList.ConvertAll(p => p.Item1))
                    CheckType(typeName);

                FunctionInfo info = new FunctionInfo(funcDecl);
                if (GlobalVariables.ContainsKey(info.Name))
                {
                    AddError("Variable with the name: " + info.Name + ", is already defined", info.NameToken);
                }
                else if (!Functions.TryAddFunction(info))
                {
                    Errors.Add(new ASTInfoError("Function " + info.Name + ", is a redefinition", info.NameToken));
                }

                m_IsInGlobalScope = false;
                base.VisitFuncDecl(funcDecl);
                m_IsInGlobalScope = true;
            }

            public override void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
            {
                CheckType(externalFuncDecl.ReturnType);
                foreach (TypeName typeName in externalFuncDecl.Parameters.ParamList.ConvertAll(p => p.Item1))
                    CheckType(typeName);

                FunctionInfo info = new FunctionInfo(externalFuncDecl);
                if (GlobalVariables.ContainsKey(info.Name))
                {
                    AddError("Variable with the name: " + info.Name + ", is already defined", info.NameToken);
                }
                else if (!Functions.TryAddFunction(info))
                {
                    Errors.Add(new ASTInfoError("Function " + info.Name + ", is a redefinition", info.NameToken));
                }

                base.VisitExternalFuncDecl(externalFuncDecl);
            }

            public override void VisitVarDecl(VarDecl varDecl)
            {
                CheckType(varDecl.Type);
                if(m_IsInGlobalScope)
                {
                    List<VariableInfo> variables = VariableInfo.FromVarDecl(varDecl);
                    foreach(VariableInfo variable in variables)
                    {
                        if(GlobalVariables.ContainsKey(variable.Name))
                        {
                            AddError("Variable with name: " + variable.Name + ", is already defined", variable.NameToken);
                        }
                        else if(Functions.ContainsFunctionWithName(variable.Name))
                        {
                            AddError("A funciton with the name: " + variable.Name + ", is aready defined", variable.NameToken);
                        }
                        else
                        {
                            GlobalVariables.Add(variable.Name, variable);
                        }
                    }
                }

                base.VisitVarDecl(varDecl);
            }

            private void AddError(string message, Token token)
            {
                Errors.Add(new ASTInfoError(message, token));
            }
        }
    }
}
