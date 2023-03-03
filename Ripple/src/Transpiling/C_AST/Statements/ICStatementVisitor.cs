using System;


namespace Ripple.Transpiling.C_AST
{
	interface ICStatementVisitor
	{
		public abstract void VisitExprStmt(ExprStmt exprStmt);
		public abstract void VisitIfStmt(IfStmt ifStmt);
		public abstract void VisitWhileStmt(WhileStmt whileStmt);
		public abstract void VisitForStmt(ForStmt forStmt);
		public abstract void VisitBlockStmt(BlockStmt blockStmt);
		public abstract void VisitVarDecl(VarDecl varDecl);
		public abstract void VisitReturnStmt(ReturnStmt returnStmt);
		public abstract void VisitBreakStmt(BreakStmt breakStmt);
		public abstract void VisitContinueStmt(ContinueStmt continueStmt);
		public abstract void VisitFuncParam(FuncParam funcParam);
		public abstract void VisitFuncDecl(FuncDecl funcDecl);
		public abstract void VisitStructMember(StructMember structMember);
		public abstract void VisitStructDecl(StructDecl structDecl);
		public abstract void VisitTypeDefStmt(TypeDefStmt typeDefStmt);
		public abstract void VisitIncludeStmt(IncludeStmt includeStmt);
		public abstract void VisitFileStmt(FileStmt fileStmt);
		public abstract void VisitProgramStmt(ProgramStmt programStmt);
	}

	interface ICStatementVisitor<T>
	{
		public abstract T VisitExprStmt(ExprStmt exprStmt);
		public abstract T VisitIfStmt(IfStmt ifStmt);
		public abstract T VisitWhileStmt(WhileStmt whileStmt);
		public abstract T VisitForStmt(ForStmt forStmt);
		public abstract T VisitBlockStmt(BlockStmt blockStmt);
		public abstract T VisitVarDecl(VarDecl varDecl);
		public abstract T VisitReturnStmt(ReturnStmt returnStmt);
		public abstract T VisitBreakStmt(BreakStmt breakStmt);
		public abstract T VisitContinueStmt(ContinueStmt continueStmt);
		public abstract T VisitFuncParam(FuncParam funcParam);
		public abstract T VisitFuncDecl(FuncDecl funcDecl);
		public abstract T VisitStructMember(StructMember structMember);
		public abstract T VisitStructDecl(StructDecl structDecl);
		public abstract T VisitTypeDefStmt(TypeDefStmt typeDefStmt);
		public abstract T VisitIncludeStmt(IncludeStmt includeStmt);
		public abstract T VisitFileStmt(FileStmt fileStmt);
		public abstract T VisitProgramStmt(ProgramStmt programStmt);
	}

	interface ICStatementVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitExprStmt(ExprStmt exprStmt, TArg arg);
		public abstract TReturn VisitIfStmt(IfStmt ifStmt, TArg arg);
		public abstract TReturn VisitWhileStmt(WhileStmt whileStmt, TArg arg);
		public abstract TReturn VisitForStmt(ForStmt forStmt, TArg arg);
		public abstract TReturn VisitBlockStmt(BlockStmt blockStmt, TArg arg);
		public abstract TReturn VisitVarDecl(VarDecl varDecl, TArg arg);
		public abstract TReturn VisitReturnStmt(ReturnStmt returnStmt, TArg arg);
		public abstract TReturn VisitBreakStmt(BreakStmt breakStmt, TArg arg);
		public abstract TReturn VisitContinueStmt(ContinueStmt continueStmt, TArg arg);
		public abstract TReturn VisitFuncParam(FuncParam funcParam, TArg arg);
		public abstract TReturn VisitFuncDecl(FuncDecl funcDecl, TArg arg);
		public abstract TReturn VisitStructMember(StructMember structMember, TArg arg);
		public abstract TReturn VisitStructDecl(StructDecl structDecl, TArg arg);
		public abstract TReturn VisitTypeDefStmt(TypeDefStmt typeDefStmt, TArg arg);
		public abstract TReturn VisitIncludeStmt(IncludeStmt includeStmt, TArg arg);
		public abstract TReturn VisitFileStmt(FileStmt fileStmt, TArg arg);
		public abstract TReturn VisitProgramStmt(ProgramStmt programStmt, TArg arg);
	}
}
