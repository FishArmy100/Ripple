using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Utils;
using Ripple.Lexing;
using Ripple.Validation.Info;
using Ripple.Validation.Errors;
using Ripple.Validation.Info.Functions;
using Ripple.Validation.Info.Checking;
using Ripple.Validation.Info.Lifetimes;

namespace Ripple.Validation
{
    class GlobalVariableFinderHelper : ASTWalkerBase
    {
        private readonly List<string> m_Primaries;
        private readonly FunctionList m_Functions;
        private readonly List<string> m_FunctionNames;
        private readonly OperatorEvaluatorLibrary m_Operators;

        public readonly List<ValidationError> Errors = new List<ValidationError>();
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
                foreach (VariableInfo info in ok.First)
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
                Errors.Add(new DefinitionError.Variable(variableInfo.NameToken.Location, true, varName));
                return;
            }

            if (GlobalVariables.ContainsKey(varName))
            {
                Errors.Add(new DefinitionError.Variable(variableInfo.NameToken.Location, true, varName));
                return;
            }

            GlobalVariables.Add(varName, variableInfo);
        }
    }
}
