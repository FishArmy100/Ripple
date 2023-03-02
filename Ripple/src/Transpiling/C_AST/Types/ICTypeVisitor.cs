using System;


namespace Ripple.Transpiling.C_AST
{
	interface ICTypeVisitor
	{
		public abstract void VisitCBasicType(CBasicType cBasicType);
		public abstract void VisitCPointer(CPointer cPointer);
		public abstract void VisitCArray(CArray cArray);
		public abstract void VisitCFuncPtr(CFuncPtr cFuncPtr);
	}

	interface ICTypeVisitor<T>
	{
		public abstract T VisitCBasicType(CBasicType cBasicType);
		public abstract T VisitCPointer(CPointer cPointer);
		public abstract T VisitCArray(CArray cArray);
		public abstract T VisitCFuncPtr(CFuncPtr cFuncPtr);
	}

	interface ICTypeVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitCBasicType(CBasicType cBasicType, TArg arg);
		public abstract TReturn VisitCPointer(CPointer cPointer, TArg arg);
		public abstract TReturn VisitCArray(CArray cArray, TArg arg);
		public abstract TReturn VisitCFuncPtr(CFuncPtr cFuncPtr, TArg arg);
	}
}
