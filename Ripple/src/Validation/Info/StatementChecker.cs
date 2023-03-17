using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Validation.Info.Statements;
using Ripple.Validation.Info.Expressions;
using Ripple.Validation.Info.Types;
using Ripple.Lexing;

namespace Ripple.Validation.Info
{
    class StatementChecker : IStatementVisitor<TypedStatement>
    {
        private readonly List<string> PrimaryTypes;
        private readonly FunctionList Functions;
        private readonly Dictionary<string, VariableInfo> GlobalVariables;
        private readonly OperatorEvaluatorLibrary OperatorLibrary;

        private bool m_IsGlobal = true;
        private bool m_IsUnsafe = false;
        private Stack<List<bool>> m_BlocksReturn = new Stack<List<bool>>();
        private readonly LocalVariableStack m_VariableStack = new LocalVariableStack();
        private readonly Stack<List<Token>> m_CurrentLifetimes = new Stack<List<Token>>();
        private TypeInfo m_CurrentReturnType = null;

        public StatementChecker(List<string> primaryTypes, FunctionList functions, Dictionary<string, VariableInfo> globalVariables, OperatorEvaluatorLibrary operatorLibrary)
        {
            PrimaryTypes = primaryTypes;
            Functions = functions;
            GlobalVariables = globalVariables;
            OperatorLibrary = operatorLibrary;
        }

        public TypedStatement VisitBlockStmt(BlockStmt blockStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitBreakStmt(BreakStmt breakStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitContinueStmt(ContinueStmt continueStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitExprStmt(ExprStmt exprStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitFileStmt(FileStmt fileStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitForStmt(ForStmt forStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitFuncDecl(FuncDecl funcDecl)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitGenericParameters(GenericParameters genericParameters)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitIfStmt(IfStmt ifStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitParameters(Parameters parameters)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitProgramStmt(ProgramStmt programStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitReturnStmt(ReturnStmt returnStmt)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitUnsafeBlock(UnsafeBlock unsafeBlock)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitVarDecl(VarDecl varDecl)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitWhereClause(WhereClause whereClause)
        {
            throw new NotImplementedException();
        }

        public TypedStatement VisitWhileStmt(WhileStmt whileStmt)
        {
            throw new NotImplementedException();
        }
    }
}
