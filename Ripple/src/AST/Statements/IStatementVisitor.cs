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
}
