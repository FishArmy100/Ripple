using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Lexing;

namespace Ripple.Validation
{
    static class AstDeclarationFinder
    {
        public static void AppendData(FileStmt fileStmt, ref AstDeclarationData declarationData)
        {
            DeclarationFinderWalker walker = new DeclarationFinderWalker();
            fileStmt.Accept(walker);
            declarationData.GlobalVariables.AddRange(walker.GlobalVariables);

            List<FunctionData> globalFunctions = walker.Functions
                .ToList()
                .ConvertAll(temp => new FunctionData(temp.Name, temp.Paramaters, temp.ReturnType));
            declarationData.GlobalFunctions.AddRange(globalFunctions);
        }

        private class TempFuncData
        {
            public Token Name = new Token();
            public Token ReturnType = new Token();
            public List<(Token, Token)> Paramaters = new List<(Token, Token)>();
        }

        private class DeclarationFinderWalker : AstWalkerBase
        {
            public readonly Stack<TempFuncData> Functions = new Stack<TempFuncData>();
            public readonly List<VariableData> GlobalVariables = new List<VariableData>();

            private bool m_IsInFunction = false;

            public override void VisitFuncDecl(FuncDecl funcDecl)
            {
                m_IsInFunction = true;
                Functions.Push(new TempFuncData());
                Functions.Peek().Name = funcDecl.Name;
                Functions.Peek().ReturnType = funcDecl.ReturnType;
                base.VisitFuncDecl(funcDecl);
                m_IsInFunction = false;
            }

            public override void VisitParameters(Parameters parameters)
            {
                foreach ((var type, var name) in parameters.ParamList)
                    Functions.Peek().Paramaters.Add((type, name));
            }

            public override void VisitVarDecl(VarDecl varDecl)
            {
                if(!m_IsInFunction)
                {
                    foreach (Token varName in varDecl.VarNames)
                        GlobalVariables.Add(new VariableData(varName, varDecl.TypeName));
                }
            }
        }
    }
}
