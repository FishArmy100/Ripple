using System;


namespace Ripple.Validation.Info.Types
{
	interface ITypeInfoVisitor
	{
		public abstract void VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo);
		public abstract void VisitPointerInfo(PointerInfo pointerInfo);
		public abstract void VisitReferenceInfo(ReferenceInfo referenceInfo);
		public abstract void VisitArrayInfo(ArrayInfo arrayInfo);
		public abstract void VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo);
	}

	interface ITypeInfoVisitor<T>
	{
		public abstract T VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo);
		public abstract T VisitPointerInfo(PointerInfo pointerInfo);
		public abstract T VisitReferenceInfo(ReferenceInfo referenceInfo);
		public abstract T VisitArrayInfo(ArrayInfo arrayInfo);
		public abstract T VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo);
	}

	interface ITypeInfoVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo, TArg arg);
		public abstract TReturn VisitPointerInfo(PointerInfo pointerInfo, TArg arg);
		public abstract TReturn VisitReferenceInfo(ReferenceInfo referenceInfo, TArg arg);
		public abstract TReturn VisitArrayInfo(ArrayInfo arrayInfo, TArg arg);
		public abstract TReturn VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo, TArg arg);
	}
	interface ITypeInfoVisitorWithArg<TArg>
	{
		public abstract void VisitBasicTypeInfo(BasicTypeInfo basicTypeInfo, TArg arg);
		public abstract void VisitPointerInfo(PointerInfo pointerInfo, TArg arg);
		public abstract void VisitReferenceInfo(ReferenceInfo referenceInfo, TArg arg);
		public abstract void VisitArrayInfo(ArrayInfo arrayInfo, TArg arg);
		public abstract void VisitFuncPtrInfo(FuncPtrInfo funcPtrInfo, TArg arg);
	}
}
