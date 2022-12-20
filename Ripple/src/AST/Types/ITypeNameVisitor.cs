using System;


namespace Ripple.AST
{
	interface ITypeNameVisitor
	{
		public abstract void VisitBasicType(BasicType basicType);
		public abstract void VisitGroupedType(GroupedType groupedType);
		public abstract void VisitPointerType(PointerType pointerType);
		public abstract void VisitReferenceType(ReferenceType referenceType);
		public abstract void VisitArrayType(ArrayType arrayType);
		public abstract void VisitFuncPtr(FuncPtr funcPtr);
	}

	interface ITypeNameVisitor<T>
	{
		public abstract T VisitBasicType(BasicType basicType);
		public abstract T VisitGroupedType(GroupedType groupedType);
		public abstract T VisitPointerType(PointerType pointerType);
		public abstract T VisitReferenceType(ReferenceType referenceType);
		public abstract T VisitArrayType(ArrayType arrayType);
		public abstract T VisitFuncPtr(FuncPtr funcPtr);
	}

	interface ITypeNameVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitBasicType(BasicType basicType, TArg arg);
		public abstract TReturn VisitGroupedType(GroupedType groupedType, TArg arg);
		public abstract TReturn VisitPointerType(PointerType pointerType, TArg arg);
		public abstract TReturn VisitReferenceType(ReferenceType referenceType, TArg arg);
		public abstract TReturn VisitArrayType(ArrayType arrayType, TArg arg);
		public abstract TReturn VisitFuncPtr(FuncPtr funcPtr, TArg arg);
	}
}
