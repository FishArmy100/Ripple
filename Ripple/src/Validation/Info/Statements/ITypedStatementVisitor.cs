

namespace Ripple.Validation.Info.Statements
{
	public interface ITypedStatementVisitor
	{
		public abstract void VisitTypedExprStmt(TypedExprStmt typedExprStmt);
		public abstract void VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt);
		public abstract void VisitTypedIfStmt(TypedIfStmt typedIfStmt);
		public abstract void VisitTypedForStmt(TypedForStmt typedForStmt);
		public abstract void VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt);
		public abstract void VisitTypedVarDecl(TypedVarDecl typedVarDecl);
		public abstract void VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt);
		public abstract void VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt);
		public abstract void VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt);
		public abstract void VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock);
		public abstract void VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl);
		public abstract void VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl);
		public abstract void VisitTypedFileStmt(TypedFileStmt typedFileStmt);
		public abstract void VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt);
	}

	public interface ITypedStatementVisitor<T>
	{
		public abstract T VisitTypedExprStmt(TypedExprStmt typedExprStmt);
		public abstract T VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt);
		public abstract T VisitTypedIfStmt(TypedIfStmt typedIfStmt);
		public abstract T VisitTypedForStmt(TypedForStmt typedForStmt);
		public abstract T VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt);
		public abstract T VisitTypedVarDecl(TypedVarDecl typedVarDecl);
		public abstract T VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt);
		public abstract T VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt);
		public abstract T VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt);
		public abstract T VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock);
		public abstract T VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl);
		public abstract T VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl);
		public abstract T VisitTypedFileStmt(TypedFileStmt typedFileStmt);
		public abstract T VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt);
	}

	public interface ITypedStatementVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitTypedExprStmt(TypedExprStmt typedExprStmt, TArg arg);
		public abstract TReturn VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt, TArg arg);
		public abstract TReturn VisitTypedIfStmt(TypedIfStmt typedIfStmt, TArg arg);
		public abstract TReturn VisitTypedForStmt(TypedForStmt typedForStmt, TArg arg);
		public abstract TReturn VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt, TArg arg);
		public abstract TReturn VisitTypedVarDecl(TypedVarDecl typedVarDecl, TArg arg);
		public abstract TReturn VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt, TArg arg);
		public abstract TReturn VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt, TArg arg);
		public abstract TReturn VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt, TArg arg);
		public abstract TReturn VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock, TArg arg);
		public abstract TReturn VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl, TArg arg);
		public abstract TReturn VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl, TArg arg);
		public abstract TReturn VisitTypedFileStmt(TypedFileStmt typedFileStmt, TArg arg);
		public abstract TReturn VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt, TArg arg);
	}
	public interface ITypedStatementVisitorWithArg<TArg>
	{
		public abstract void VisitTypedExprStmt(TypedExprStmt typedExprStmt, TArg arg);
		public abstract void VisitTypedBlockStmt(TypedBlockStmt typedBlockStmt, TArg arg);
		public abstract void VisitTypedIfStmt(TypedIfStmt typedIfStmt, TArg arg);
		public abstract void VisitTypedForStmt(TypedForStmt typedForStmt, TArg arg);
		public abstract void VisitTypedWhileStmt(TypedWhileStmt typedWhileStmt, TArg arg);
		public abstract void VisitTypedVarDecl(TypedVarDecl typedVarDecl, TArg arg);
		public abstract void VisitTypedReturnStmt(TypedReturnStmt typedReturnStmt, TArg arg);
		public abstract void VisitTypedContinueStmt(TypedContinueStmt typedContinueStmt, TArg arg);
		public abstract void VisitTypedBreakStmt(TypedBreakStmt typedBreakStmt, TArg arg);
		public abstract void VisitTypedUnsafeBlock(TypedUnsafeBlock typedUnsafeBlock, TArg arg);
		public abstract void VisitTypedFuncDecl(TypedFuncDecl typedFuncDecl, TArg arg);
		public abstract void VisitTypedExternalFuncDecl(TypedExternalFuncDecl typedExternalFuncDecl, TArg arg);
		public abstract void VisitTypedFileStmt(TypedFileStmt typedFileStmt, TArg arg);
		public abstract void VisitTypedProgramStmt(TypedProgramStmt typedProgramStmt, TArg arg);
	}
}
