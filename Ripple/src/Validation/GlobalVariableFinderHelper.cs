using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Utils;
using Ripple.Lexing;
using Ripple.Validation.Info;

namespace Ripple.Validation
{
    class GlobalVariableFinderHelper : ASTWalkerBase
    {
        private readonly List<string> m_Primaries;
        private readonly FunctionList m_Functions;
        private readonly List<string> m_FunctionNames;
        private readonly OperatorEvaluatorLibrary m_Operators;

        public readonly List<ASTInfoError> Errors = new List<ASTInfoError>();
        public readonly Dictionary<string, VariableInfo> GlobalVariables = new Dictionary<string, VariableInfo>();

        public GlobalVariableFinderHelper(ProgramStmt ast, List<string> primaries, FunctionList functions, OperatorEvaluatorLibrary operators)
        {
            m_Primaries = primaries;
            m_Functions = functions;
            m_FunctionNames = functions.GetFunctionNames();
            m_Operators = operators;
            ast.Accept(this);
        }

        public override void VisitFuncDecl(FuncDecl funcDecl) { } // not implemented, so only global variables are visited

        public override void VisitVarDecl(VarDecl varDecl)
        {
            LocalVariableStack variableStack = new LocalVariableStack();
            SafetyContext safetyContext = new SafetyContext(!varDecl.UnsafeToken.HasValue);
            ExpressionCheckerVisitor visitor = new ExpressionCheckerVisitor(variableStack, m_Functions, m_Operators, GlobalVariables, safetyContext, m_Primaries, new List<string>());
            var result = VariableInfo.FromVarDecl(varDecl, visitor, m_Primaries, new List<string>(), LifetimeInfo.Static, safetyContext);
            result.Match(ok =>
            {
                foreach (VariableInfo info in ok)
                    TryAddVariableInfo(info);
            },
            fail =>
            {
                Errors.AddRange(fail);
            });
        }

        private void TryAddVariableInfo(VariableInfo variableInfo)
        {
            string varName = variableInfo.Name;
            if (m_FunctionNames.Contains(varName))
            {
                AddError("Variable name '" + varName + "' is already used as a function name.", variableInfo.NameToken);
                return;
            }

            if (GlobalVariables.ContainsKey(varName))
            {
                AddError("Global variable with name '" + varName + "' already exists.", variableInfo.NameToken);
                return;
            }

            GlobalVariables.Add(varName, variableInfo);
        }

        private void AddError(string name, Token token)
        {
            Errors.Add(new ASTInfoError(name, token));
        }
    }
}
