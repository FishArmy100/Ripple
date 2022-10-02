using System;


namespace Ripple.AST
{
	interface ITypeNameVisitor
	{
		public abstract void VisitGroupedType(GroupedType groupedType);
		public abstract void VisitPointerType(PointerType pointerType);
		public abstract void VisitReferenceType(ReferenceType referenceType);
		public abstract void VisitFuncPtr(FuncPtr funcPtr);
	}

	interface ITypeNameVisitor<T>
	{
		public abstract T VisitGroupedType(GroupedType groupedType);
		public abstract T VisitPointerType(PointerType pointerType);
		public abstract T VisitReferenceType(ReferenceType referenceType);
		public abstract T VisitFuncPtr(FuncPtr funcPtr);
	}
}
