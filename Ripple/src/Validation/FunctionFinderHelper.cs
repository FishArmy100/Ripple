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

namespace Ripple.Validation
{
    class FunctionFinderHelper : ASTWalkerBase
    {
        private readonly List<string> m_Primaries;
        private readonly List<string> m_GlobalVariableNames = new List<string>();

        public List<ValidationError> Errors { get; private set; } = new List<ValidationError>();
        public FunctionList Functions { get; private set; }

        public FunctionFinderHelper(ProgramStmt ast, List<string> primaries, FunctionList additionalFunctions)
        {
            Functions = additionalFunctions;
            m_Primaries = primaries;
            ast.Accept(this);
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
            if (m_GlobalVariableNames.Contains(info.Name))
            {
                Errors.Add(new DefinitionError.Variable(info.NameToken.Location, true, info.Name));
            }
            else if (!Functions.TryAddFunction(info))
            {
                Errors.Add(new DefinitionError.Function(info.NameToken.Location, true, info.Name));
            }
        }
    }
}
