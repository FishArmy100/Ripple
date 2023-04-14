

namespace Ripple.Transpiling.C_AST
{
	public interface ICStatementVisitor
	{
		public abstract void VisitCExprStmt(CExprStmt cExprStmt);
		public abstract void VisitCIfStmt(CIfStmt cIfStmt);
		public abstract void VisitCWhileStmt(CWhileStmt cWhileStmt);
		public abstract void VisitCForStmt(CForStmt cForStmt);
		public abstract void VisitCBlockStmt(CBlockStmt cBlockStmt);
		public abstract void VisitCVarDecl(CVarDecl cVarDecl);
		public abstract void VisitCReturnStmt(CReturnStmt cReturnStmt);
		public abstract void VisitCBreakStmt(CBreakStmt cBreakStmt);
		public abstract void VisitCContinueStmt(CContinueStmt cContinueStmt);
		public abstract void VisitCFuncDef(CFuncDef cFuncDef);
		public abstract void VisitCFuncDecl(CFuncDecl cFuncDecl);
		public abstract void VisitCStructMember(CStructMember cStructMember);
		public abstract void VisitCStructDef(CStructDef cStructDef);
		public abstract void VisitCStructDecl(CStructDecl cStructDecl);
		public abstract void VisitCTypeDefStmt(CTypeDefStmt cTypeDefStmt);
		public abstract void VisitCIncludeStmt(CIncludeStmt cIncludeStmt);
		public abstract void VisitCFileStmt(CFileStmt cFileStmt);
	}

	public interface ICStatementVisitor<T>
	{
		public abstract T VisitCExprStmt(CExprStmt cExprStmt);
		public abstract T VisitCIfStmt(CIfStmt cIfStmt);
		public abstract T VisitCWhileStmt(CWhileStmt cWhileStmt);
		public abstract T VisitCForStmt(CForStmt cForStmt);
		public abstract T VisitCBlockStmt(CBlockStmt cBlockStmt);
		public abstract T VisitCVarDecl(CVarDecl cVarDecl);
		public abstract T VisitCReturnStmt(CReturnStmt cReturnStmt);
		public abstract T VisitCBreakStmt(CBreakStmt cBreakStmt);
		public abstract T VisitCContinueStmt(CContinueStmt cContinueStmt);
		public abstract T VisitCFuncDef(CFuncDef cFuncDef);
		public abstract T VisitCFuncDecl(CFuncDecl cFuncDecl);
		public abstract T VisitCStructMember(CStructMember cStructMember);
		public abstract T VisitCStructDef(CStructDef cStructDef);
		public abstract T VisitCStructDecl(CStructDecl cStructDecl);
		public abstract T VisitCTypeDefStmt(CTypeDefStmt cTypeDefStmt);
		public abstract T VisitCIncludeStmt(CIncludeStmt cIncludeStmt);
		public abstract T VisitCFileStmt(CFileStmt cFileStmt);
	}

	public interface ICStatementVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitCExprStmt(CExprStmt cExprStmt, TArg arg);
		public abstract TReturn VisitCIfStmt(CIfStmt cIfStmt, TArg arg);
		public abstract TReturn VisitCWhileStmt(CWhileStmt cWhileStmt, TArg arg);
		public abstract TReturn VisitCForStmt(CForStmt cForStmt, TArg arg);
		public abstract TReturn VisitCBlockStmt(CBlockStmt cBlockStmt, TArg arg);
		public abstract TReturn VisitCVarDecl(CVarDecl cVarDecl, TArg arg);
		public abstract TReturn VisitCReturnStmt(CReturnStmt cReturnStmt, TArg arg);
		public abstract TReturn VisitCBreakStmt(CBreakStmt cBreakStmt, TArg arg);
		public abstract TReturn VisitCContinueStmt(CContinueStmt cContinueStmt, TArg arg);
		public abstract TReturn VisitCFuncDef(CFuncDef cFuncDef, TArg arg);
		public abstract TReturn VisitCFuncDecl(CFuncDecl cFuncDecl, TArg arg);
		public abstract TReturn VisitCStructMember(CStructMember cStructMember, TArg arg);
		public abstract TReturn VisitCStructDef(CStructDef cStructDef, TArg arg);
		public abstract TReturn VisitCStructDecl(CStructDecl cStructDecl, TArg arg);
		public abstract TReturn VisitCTypeDefStmt(CTypeDefStmt cTypeDefStmt, TArg arg);
		public abstract TReturn VisitCIncludeStmt(CIncludeStmt cIncludeStmt, TArg arg);
		public abstract TReturn VisitCFileStmt(CFileStmt cFileStmt, TArg arg);
	}
	public interface ICStatementVisitorWithArg<TArg>
	{
		public abstract void VisitCExprStmt(CExprStmt cExprStmt, TArg arg);
		public abstract void VisitCIfStmt(CIfStmt cIfStmt, TArg arg);
		public abstract void VisitCWhileStmt(CWhileStmt cWhileStmt, TArg arg);
		public abstract void VisitCForStmt(CForStmt cForStmt, TArg arg);
		public abstract void VisitCBlockStmt(CBlockStmt cBlockStmt, TArg arg);
		public abstract void VisitCVarDecl(CVarDecl cVarDecl, TArg arg);
		public abstract void VisitCReturnStmt(CReturnStmt cReturnStmt, TArg arg);
		public abstract void VisitCBreakStmt(CBreakStmt cBreakStmt, TArg arg);
		public abstract void VisitCContinueStmt(CContinueStmt cContinueStmt, TArg arg);
		public abstract void VisitCFuncDef(CFuncDef cFuncDef, TArg arg);
		public abstract void VisitCFuncDecl(CFuncDecl cFuncDecl, TArg arg);
		public abstract void VisitCStructMember(CStructMember cStructMember, TArg arg);
		public abstract void VisitCStructDef(CStructDef cStructDef, TArg arg);
		public abstract void VisitCStructDecl(CStructDecl cStructDecl, TArg arg);
		public abstract void VisitCTypeDefStmt(CTypeDefStmt cTypeDefStmt, TArg arg);
		public abstract void VisitCIncludeStmt(CIncludeStmt cIncludeStmt, TArg arg);
		public abstract void VisitCFileStmt(CFileStmt cFileStmt, TArg arg);
	}
}
