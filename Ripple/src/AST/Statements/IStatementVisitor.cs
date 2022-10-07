using System;


namespace Ripple.AST
{
	interface IStatementVisitor
	{
		public abstract void VisitExprStmt(ExprStmt exprStmt);
		public abstract void VisitBlockStmt(BlockStmt blockStmt);
		public abstract void VisitIfStmt(IfStmt ifStmt);
		public abstract void VisitForStmt(ForStmt forStmt);
		public abstract void VisitWhileStmt(WhileStmt whileStmt);
		public abstract void VisitVarDecl(VarDecl varDecl);
		public abstract void VisitReturnStmt(ReturnStmt returnStmt);
		public abstract void VisitContinueStmt(ContinueStmt continueStmt);
		public abstract void VisitBreakStmt(BreakStmt breakStmt);
		public abstract void VisitParameters(Parameters parameters);
		public abstract void VisitFuncDecl(FuncDecl funcDecl);
		public abstract void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl);
		public abstract void VisitFileStmt(FileStmt fileStmt);
		public abstract void VisitProgram(Program program);
	}

	interface IStatementVisitor<T>
	{
		public abstract T VisitExprStmt(ExprStmt exprStmt);
		public abstract T VisitBlockStmt(BlockStmt blockStmt);
		public abstract T VisitIfStmt(IfStmt ifStmt);
		public abstract T VisitForStmt(ForStmt forStmt);
		public abstract T VisitWhileStmt(WhileStmt whileStmt);
		public abstract T VisitVarDecl(VarDecl varDecl);
		public abstract T VisitReturnStmt(ReturnStmt returnStmt);
		public abstract T VisitContinueStmt(ContinueStmt continueStmt);
		public abstract T VisitBreakStmt(BreakStmt breakStmt);
		public abstract T VisitParameters(Parameters parameters);
		public abstract T VisitFuncDecl(FuncDecl funcDecl);
		public abstract T VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl);
		public abstract T VisitFileStmt(FileStmt fileStmt);
		public abstract T VisitProgram(Program program);
	}
}
