using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Info;
using Ripple.AST.Utils;
using Ripple.Lexing;
using Ripple.Utils;

namespace Ripple.Validation
{
    class BasicTypeCheckStep : IStatementVisitor
    {
        private readonly ASTInfo m_ASTInfo;
        private readonly LocalVariableStack m_LocalVariables = new LocalVariableStack();
        public readonly List<ValidationError> Errors = new List<ValidationError>();

        private int m_LoopCount = 0;
        private FuncDecl m_CurrentFunc = null;
        private bool m_DoesFunctionReturn = false;

        public BasicTypeCheckStep(ASTInfo astInfo)
        {
            m_ASTInfo = astInfo;
            m_ASTInfo.AST.Accept(this);
        }

        public void VisitBlockStmt(BlockStmt blockStmt)
        {
            m_LocalVariables.PushScope();
            foreach (var stmt in blockStmt.Statements)
                stmt.Accept(this);
            m_LocalVariables.PopScope();
        }

        public void VisitBreakStmt(BreakStmt breakStmt)
        {
            if (m_LoopCount <= 0)
                AddError("Break statement must be inside a loop.", breakStmt.BreakToken);
        }

        public void VisitContinueStmt(ContinueStmt continueStmt)
        {
            if (m_LoopCount <= 0)
                AddError("Continue statement must be inside a loop.", continueStmt.ContinueToken);
        }

        public void VisitExprStmt(ExprStmt exprStmt)
        {
            CheckExpression(exprStmt.Expr);
        }

        public void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl) { }

        public void VisitFileStmt(FileStmt fileStmt)
        {
            foreach (Statement statement in fileStmt.Statements)
                statement.Accept(this);
        }

        public void VisitForStmt(ForStmt forStmt)
        {
            m_LocalVariables.PushScope();
            forStmt.Init.Match(init => init.Accept(this));
            forStmt.Condition.Match(cond =>
            {
                VerifyReturnBool(cond, forStmt.ForTok);
            });
            forStmt.Iter.Match(iter => CheckExpression(iter));

            m_LoopCount++;
            forStmt.Body.Accept(this);
            m_LoopCount--;

            m_LocalVariables.PopScope();
        }

        private void VerifyReturnBool(Expression cond, Token tokenForError)
        {
            CheckExpression(cond).Match(type =>
            {
                if (!type.Equals(RipplePrimitives.Bool))
                    AddError("Condition expression must evaluate to a bool.", tokenForError);
            });
        }

        public void VisitFuncDecl(FuncDecl funcDecl)
        {
            m_CurrentFunc = funcDecl;
            m_LocalVariables.PushScope();

            foreach ((TypeName type, Token name) in funcDecl.Param.ParamList)
                m_LocalVariables.AddVariable(new VariableInfo(name, TypeInfo.FromASTType(type), false));

            funcDecl.Body.Accept(this);
            m_LocalVariables.PopScope();
            m_CurrentFunc = null;
        }

        public void VisitGenericParameters(GenericParameters genericParameters) { }

        public void VisitIfStmt(IfStmt ifStmt)
        {
            VerifyReturnBool(ifStmt.Expr, ifStmt.IfTok);

            m_LocalVariables.PushScope();
            ifStmt.Body.Accept(this);
            m_LocalVariables.PopScope();

            m_LocalVariables.PushScope();
            ifStmt.ElseBody.Match(body => body.Accept(this));
            m_LocalVariables.PopScope();
        }

        public void VisitParameters(Parameters parameters) { }

        public void VisitProgramStmt(ProgramStmt programStmt)
        {
            foreach (FileStmt file in programStmt.Files)
                file.Accept(this);
        }

        public void VisitReturnStmt(ReturnStmt returnStmt)
        {
            
        }

        public void VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            foreach (Statement statement in unsafeBlock.Statements)
                statement.Accept(this);
        }

        public void VisitVarDecl(VarDecl varDecl)
        {
            TypeInfo varType = TypeInfo.FromASTType(varDecl.Type);
            TypeInfo varTypeChecker = varType.ChangeMutable(false);
            CheckExpression(varDecl.Expr).Match(ok =>
            {
                if (!varTypeChecker.Equals(ok.ChangeMutable(false)))
                {
                    AddError("Variable of type: " + varType.ToPrettyString() +
                             " cannot be assigned to a expression evaluating to type: " +
                             ok.ToPrettyString(), varDecl.VarNames[0]);
                }
                else
                {
                    foreach (var info in VariableInfo.FromVarDecl(varDecl))
                        m_LocalVariables.AddVariable(info);
                }
            });
        }

        public void VisitWhereClause(WhereClause whereClause)
        {
            throw new NotImplementedException();
        }

        public void VisitWhileStmt(WhileStmt whileStmt)
        {
            VerifyReturnBool(whileStmt.Condition, whileStmt.WhileToken);
            m_LoopCount++;
            m_LocalVariables.PushScope();
            whileStmt.Body.Accept(this);
            m_LocalVariables.PopScope();
            m_LoopCount--;
        }

        private Option<TypeInfo> CheckExpression(Expression expression)
        {
            var result = TypeInfoHelper.GetTypeOfExpression(m_ASTInfo, m_LocalVariables, expression);
            Option<TypeInfo> info = new Option<TypeInfo>();
            result.Match(ok => { info = ok; }, fail => AddError(fail));
            return info;
        }

        private void AddError(TypeInfoHelper.Error error) => AddError(error.Message, error.Token);

        private void AddError(string message, Token token)
        {
            Errors.Add(new ValidationError(message, token));
        }

        private bool TryGetVariable(string name, out VariableInfo info)
        {
            if (m_LocalVariables.TryGetVariable(name, out info))
                return true;
            else if (m_ASTInfo.GlobalVariables.TryGetValue(name, out info))
                return true;

            return false;
        }
    }
}
