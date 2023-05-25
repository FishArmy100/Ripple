

namespace Ripple.AST
{
	public interface IStatementVisitor
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
		public abstract void VisitGenericParameters(GenericParameters genericParameters);
		public abstract void VisitWhereClause(WhereClause whereClause);
		public abstract void VisitUnsafeBlock(UnsafeBlock unsafeBlock);
		public abstract void VisitFuncDecl(FuncDecl funcDecl);
		public abstract void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl);
		public abstract void VisitConstructorDecl(ConstructorDecl constructorDecl);
		public abstract void VisitDestructorDecl(DestructorDecl destructorDecl);
		public abstract void VisitThisFunctionParameter(ThisFunctionParameter thisFunctionParameter);
		public abstract void VisitMemberFunctionParameters(MemberFunctionParameters memberFunctionParameters);
		public abstract void VisitMemberFunctionDecl(MemberFunctionDecl memberFunctionDecl);
		public abstract void VisitMemberDecl(MemberDecl memberDecl);
		public abstract void VisitClassDecl(ClassDecl classDecl);
		public abstract void VisitFileStmt(FileStmt fileStmt);
		public abstract void VisitProgramStmt(ProgramStmt programStmt);
	}

	public interface IStatementVisitor<T>
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
		public abstract T VisitGenericParameters(GenericParameters genericParameters);
		public abstract T VisitWhereClause(WhereClause whereClause);
		public abstract T VisitUnsafeBlock(UnsafeBlock unsafeBlock);
		public abstract T VisitFuncDecl(FuncDecl funcDecl);
		public abstract T VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl);
		public abstract T VisitConstructorDecl(ConstructorDecl constructorDecl);
		public abstract T VisitDestructorDecl(DestructorDecl destructorDecl);
		public abstract T VisitThisFunctionParameter(ThisFunctionParameter thisFunctionParameter);
		public abstract T VisitMemberFunctionParameters(MemberFunctionParameters memberFunctionParameters);
		public abstract T VisitMemberFunctionDecl(MemberFunctionDecl memberFunctionDecl);
		public abstract T VisitMemberDecl(MemberDecl memberDecl);
		public abstract T VisitClassDecl(ClassDecl classDecl);
		public abstract T VisitFileStmt(FileStmt fileStmt);
		public abstract T VisitProgramStmt(ProgramStmt programStmt);
	}

	public interface IStatementVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitExprStmt(ExprStmt exprStmt, TArg arg);
		public abstract TReturn VisitBlockStmt(BlockStmt blockStmt, TArg arg);
		public abstract TReturn VisitIfStmt(IfStmt ifStmt, TArg arg);
		public abstract TReturn VisitForStmt(ForStmt forStmt, TArg arg);
		public abstract TReturn VisitWhileStmt(WhileStmt whileStmt, TArg arg);
		public abstract TReturn VisitVarDecl(VarDecl varDecl, TArg arg);
		public abstract TReturn VisitReturnStmt(ReturnStmt returnStmt, TArg arg);
		public abstract TReturn VisitContinueStmt(ContinueStmt continueStmt, TArg arg);
		public abstract TReturn VisitBreakStmt(BreakStmt breakStmt, TArg arg);
		public abstract TReturn VisitParameters(Parameters parameters, TArg arg);
		public abstract TReturn VisitGenericParameters(GenericParameters genericParameters, TArg arg);
		public abstract TReturn VisitWhereClause(WhereClause whereClause, TArg arg);
		public abstract TReturn VisitUnsafeBlock(UnsafeBlock unsafeBlock, TArg arg);
		public abstract TReturn VisitFuncDecl(FuncDecl funcDecl, TArg arg);
		public abstract TReturn VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl, TArg arg);
		public abstract TReturn VisitConstructorDecl(ConstructorDecl constructorDecl, TArg arg);
		public abstract TReturn VisitDestructorDecl(DestructorDecl destructorDecl, TArg arg);
		public abstract TReturn VisitThisFunctionParameter(ThisFunctionParameter thisFunctionParameter, TArg arg);
		public abstract TReturn VisitMemberFunctionParameters(MemberFunctionParameters memberFunctionParameters, TArg arg);
		public abstract TReturn VisitMemberFunctionDecl(MemberFunctionDecl memberFunctionDecl, TArg arg);
		public abstract TReturn VisitMemberDecl(MemberDecl memberDecl, TArg arg);
		public abstract TReturn VisitClassDecl(ClassDecl classDecl, TArg arg);
		public abstract TReturn VisitFileStmt(FileStmt fileStmt, TArg arg);
		public abstract TReturn VisitProgramStmt(ProgramStmt programStmt, TArg arg);
	}
	public interface IStatementVisitorWithArg<TArg>
	{
		public abstract void VisitExprStmt(ExprStmt exprStmt, TArg arg);
		public abstract void VisitBlockStmt(BlockStmt blockStmt, TArg arg);
		public abstract void VisitIfStmt(IfStmt ifStmt, TArg arg);
		public abstract void VisitForStmt(ForStmt forStmt, TArg arg);
		public abstract void VisitWhileStmt(WhileStmt whileStmt, TArg arg);
		public abstract void VisitVarDecl(VarDecl varDecl, TArg arg);
		public abstract void VisitReturnStmt(ReturnStmt returnStmt, TArg arg);
		public abstract void VisitContinueStmt(ContinueStmt continueStmt, TArg arg);
		public abstract void VisitBreakStmt(BreakStmt breakStmt, TArg arg);
		public abstract void VisitParameters(Parameters parameters, TArg arg);
		public abstract void VisitGenericParameters(GenericParameters genericParameters, TArg arg);
		public abstract void VisitWhereClause(WhereClause whereClause, TArg arg);
		public abstract void VisitUnsafeBlock(UnsafeBlock unsafeBlock, TArg arg);
		public abstract void VisitFuncDecl(FuncDecl funcDecl, TArg arg);
		public abstract void VisitExternalFuncDecl(ExternalFuncDecl externalFuncDecl, TArg arg);
		public abstract void VisitConstructorDecl(ConstructorDecl constructorDecl, TArg arg);
		public abstract void VisitDestructorDecl(DestructorDecl destructorDecl, TArg arg);
		public abstract void VisitThisFunctionParameter(ThisFunctionParameter thisFunctionParameter, TArg arg);
		public abstract void VisitMemberFunctionParameters(MemberFunctionParameters memberFunctionParameters, TArg arg);
		public abstract void VisitMemberFunctionDecl(MemberFunctionDecl memberFunctionDecl, TArg arg);
		public abstract void VisitMemberDecl(MemberDecl memberDecl, TArg arg);
		public abstract void VisitClassDecl(ClassDecl classDecl, TArg arg);
		public abstract void VisitFileStmt(FileStmt fileStmt, TArg arg);
		public abstract void VisitProgramStmt(ProgramStmt programStmt, TArg arg);
	}
}
