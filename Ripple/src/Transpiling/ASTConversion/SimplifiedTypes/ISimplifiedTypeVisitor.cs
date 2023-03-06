using System;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	interface ISimplifiedTypeVisitor
	{
		public abstract void VisitSBasicType(SBasicType sBasicType);
		public abstract void VisitSPointer(SPointer sPointer);
		public abstract void VisitSReference(SReference sReference);
		public abstract void VisitSArray(SArray sArray);
		public abstract void VisitSFuncPtr(SFuncPtr sFuncPtr);
	}

	interface ISimplifiedTypeVisitor<T>
	{
		public abstract T VisitSBasicType(SBasicType sBasicType);
		public abstract T VisitSPointer(SPointer sPointer);
		public abstract T VisitSReference(SReference sReference);
		public abstract T VisitSArray(SArray sArray);
		public abstract T VisitSFuncPtr(SFuncPtr sFuncPtr);
	}

	interface ISimplifiedTypeVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitSBasicType(SBasicType sBasicType, TArg arg);
		public abstract TReturn VisitSPointer(SPointer sPointer, TArg arg);
		public abstract TReturn VisitSReference(SReference sReference, TArg arg);
		public abstract TReturn VisitSArray(SArray sArray, TArg arg);
		public abstract TReturn VisitSFuncPtr(SFuncPtr sFuncPtr, TArg arg);
	}
	interface ISimplifiedTypeVisitorWithArg<TArg>
	{
		public abstract void VisitSBasicType(SBasicType sBasicType, TArg arg);
		public abstract void VisitSPointer(SPointer sPointer, TArg arg);
		public abstract void VisitSReference(SReference sReference, TArg arg);
		public abstract void VisitSArray(SArray sArray, TArg arg);
		public abstract void VisitSFuncPtr(SFuncPtr sFuncPtr, TArg arg);
	}
}
