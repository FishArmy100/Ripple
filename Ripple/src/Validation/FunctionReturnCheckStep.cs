using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.AST.Utils;

namespace Ripple.Validation
{
    class FunctionReturnCheckStep : ASTWalkerBase
    {
        public readonly List<ValidationError> Errors = new List<ValidationError>();
        private bool m_PreviousBlockReturns = false;

        public FunctionReturnCheckStep(ASTInfo info)
        {
            info.AST.Accept(this);
        }

        public override void VisitForStmt(ForStmt forStmt)
        {
            m_PreviousBlockReturns = false;
        }

        public override void VisitWhileStmt(WhileStmt whileStmt)
        {
            m_PreviousBlockReturns = false;
        }

        public override void VisitFuncDecl(FuncDecl funcDecl)
        {
            m_PreviousBlockReturns = false;
            TypeInfo returnType = TypeInfo.FromASTType(funcDecl.ReturnType);

            if(!returnType.Equals(RipplePrimitives.Void))
            {
                funcDecl.Body.Accept(this);
                if (!m_PreviousBlockReturns)
                    Errors.Add(new ValidationError("Not all paths return a value.", funcDecl.Name));
            }
        }

        public override void VisitIfStmt(IfStmt ifStmt)
        {
            if(ifStmt.ElseToken.HasValue)
            {
                ifStmt.Body.Accept(this);
                bool ifReturns = m_PreviousBlockReturns;

                ifStmt.ElseBody.Value.Accept(this);
                bool elseReturns = m_PreviousBlockReturns;

                m_PreviousBlockReturns = ifReturns && elseReturns;
            }
            else
            {
                m_PreviousBlockReturns = false;
            }
        }

        public override void VisitReturnStmt(ReturnStmt returnStmt)
        {
            m_PreviousBlockReturns = true;
        }
    }
}
