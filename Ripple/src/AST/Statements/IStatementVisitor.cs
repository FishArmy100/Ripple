using System;


namespace Ripple.AST
{
	interface IStatementVisitor
	{
		public abstract void VisitExprStmt(ExprStmt exprStmt);
		public abstract void VisitBlockStmt(BlockStmt blockStmt);
		public abstract void VisitIfStmt(IfStmt ifStmt);
		public abstract void VisitForStmt(ForStmt forStmt);
		public abstract void VisitVarDecl(VarDecl varDecl);
		public abstract void VisitReturnStmt(ReturnStmt returnStmt);
		public abstract void VisitParameters(Parameters parameters);
		public abstract void VisitFuncDecl(FuncDecl funcDecl);
		public abstract void VisitFileStmt(FileStmt fileStmt);
	}

	interface IStatementVisitor<T>
	{
		public abstract T VisitExprStmt(ExprStmt exprStmt);
		public abstract T VisitBlockStmt(BlockStmt blockStmt);
		public abstract T VisitIfStmt(IfStmt ifStmt);
		public abstract T VisitForStmt(ForStmt forStmt);
		public abstract T VisitVarDecl(VarDecl varDecl);
		public abstract T VisitReturnStmt(ReturnStmt returnStmt);
		public abstract T VisitParameters(Parameters parameters);
		public abstract T VisitFuncDecl(FuncDecl funcDecl);
		public abstract T VisitFileStmt(FileStmt fileStmt);
	}
}
