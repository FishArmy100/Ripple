using System;


namespace Ripple.Transpiling.C_AST
{
	interface ICTypeVisitor
	{
		public abstract void VisitBasic(Basic basic);
		public abstract void VisitPointer(Pointer pointer);
		public abstract void VisitArray(Array array);
		public abstract void VisitFuncPtr(FuncPtr funcPtr);
	}

	interface ICTypeVisitor<T>
	{
		public abstract T VisitBasic(Basic basic);
		public abstract T VisitPointer(Pointer pointer);
		public abstract T VisitArray(Array array);
		public abstract T VisitFuncPtr(FuncPtr funcPtr);
	}

	interface ICTypeVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitBasic(Basic basic, TArg arg);
		public abstract TReturn VisitPointer(Pointer pointer, TArg arg);
		public abstract TReturn VisitArray(Array array, TArg arg);
		public abstract TReturn VisitFuncPtr(FuncPtr funcPtr, TArg arg);
	}
}
