using System;


namespace Ripple.Transpiling.C_AST
{
	interface ICExpressionVisitor
	{
		public abstract void VisitCBinary(CBinary cBinary);
		public abstract void VisitCUnary(CUnary cUnary);
		public abstract void VisitCIndex(CIndex cIndex);
		public abstract void VisitCCall(CCall cCall);
		public abstract void VisitCCast(CCast cCast);
		public abstract void VisitCIdentifier(CIdentifier cIdentifier);
		public abstract void VisitCSizeOf(CSizeOf cSizeOf);
		public abstract void VisitCMemberAccess(CMemberAccess cMemberAccess);
		public abstract void VisitCLiteral(CLiteral cLiteral);
		public abstract void VisitCInitalizerList(CInitalizerList cInitalizerList);
	}

	interface ICExpressionVisitor<T>
	{
		public abstract T VisitCBinary(CBinary cBinary);
		public abstract T VisitCUnary(CUnary cUnary);
		public abstract T VisitCIndex(CIndex cIndex);
		public abstract T VisitCCall(CCall cCall);
		public abstract T VisitCCast(CCast cCast);
		public abstract T VisitCIdentifier(CIdentifier cIdentifier);
		public abstract T VisitCSizeOf(CSizeOf cSizeOf);
		public abstract T VisitCMemberAccess(CMemberAccess cMemberAccess);
		public abstract T VisitCLiteral(CLiteral cLiteral);
		public abstract T VisitCInitalizerList(CInitalizerList cInitalizerList);
	}

	interface ICExpressionVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitCBinary(CBinary cBinary, TArg arg);
		public abstract TReturn VisitCUnary(CUnary cUnary, TArg arg);
		public abstract TReturn VisitCIndex(CIndex cIndex, TArg arg);
		public abstract TReturn VisitCCall(CCall cCall, TArg arg);
		public abstract TReturn VisitCCast(CCast cCast, TArg arg);
		public abstract TReturn VisitCIdentifier(CIdentifier cIdentifier, TArg arg);
		public abstract TReturn VisitCSizeOf(CSizeOf cSizeOf, TArg arg);
		public abstract TReturn VisitCMemberAccess(CMemberAccess cMemberAccess, TArg arg);
		public abstract TReturn VisitCLiteral(CLiteral cLiteral, TArg arg);
		public abstract TReturn VisitCInitalizerList(CInitalizerList cInitalizerList, TArg arg);
	}
	interface ICExpressionVisitorWithArg<TArg>
	{
		public abstract void VisitCBinary(CBinary cBinary, TArg arg);
		public abstract void VisitCUnary(CUnary cUnary, TArg arg);
		public abstract void VisitCIndex(CIndex cIndex, TArg arg);
		public abstract void VisitCCall(CCall cCall, TArg arg);
		public abstract void VisitCCast(CCast cCast, TArg arg);
		public abstract void VisitCIdentifier(CIdentifier cIdentifier, TArg arg);
		public abstract void VisitCSizeOf(CSizeOf cSizeOf, TArg arg);
		public abstract void VisitCMemberAccess(CMemberAccess cMemberAccess, TArg arg);
		public abstract void VisitCLiteral(CLiteral cLiteral, TArg arg);
		public abstract void VisitCInitalizerList(CInitalizerList cInitalizerList, TArg arg);
	}
}
