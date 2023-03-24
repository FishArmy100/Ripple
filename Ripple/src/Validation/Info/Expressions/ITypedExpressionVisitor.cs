using System;


namespace Ripple.Validation.Info.Expressions
{
	interface ITypedExpressionVisitor
	{
		public abstract void VisitTypedIdentifier(TypedIdentifier typedIdentifier);
		public abstract void VisitTypedInitalizerList(TypedInitalizerList typedInitalizerList);
		public abstract void VisitTypedLiteral(TypedLiteral typedLiteral);
		public abstract void VisitTypedSizeOf(TypedSizeOf typedSizeOf);
		public abstract void VisitTypedCall(TypedCall typedCall);
		public abstract void VisitTypedIndex(TypedIndex typedIndex);
		public abstract void VisitTypedCast(TypedCast typedCast);
		public abstract void VisitTypedBinary(TypedBinary typedBinary);
		public abstract void VisitTypedUnary(TypedUnary typedUnary);
	}

	interface ITypedExpressionVisitor<T>
	{
		public abstract T VisitTypedIdentifier(TypedIdentifier typedIdentifier);
		public abstract T VisitTypedInitalizerList(TypedInitalizerList typedInitalizerList);
		public abstract T VisitTypedLiteral(TypedLiteral typedLiteral);
		public abstract T VisitTypedSizeOf(TypedSizeOf typedSizeOf);
		public abstract T VisitTypedCall(TypedCall typedCall);
		public abstract T VisitTypedIndex(TypedIndex typedIndex);
		public abstract T VisitTypedCast(TypedCast typedCast);
		public abstract T VisitTypedBinary(TypedBinary typedBinary);
		public abstract T VisitTypedUnary(TypedUnary typedUnary);
	}

	interface ITypedExpressionVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitTypedIdentifier(TypedIdentifier typedIdentifier, TArg arg);
		public abstract TReturn VisitTypedInitalizerList(TypedInitalizerList typedInitalizerList, TArg arg);
		public abstract TReturn VisitTypedLiteral(TypedLiteral typedLiteral, TArg arg);
		public abstract TReturn VisitTypedSizeOf(TypedSizeOf typedSizeOf, TArg arg);
		public abstract TReturn VisitTypedCall(TypedCall typedCall, TArg arg);
		public abstract TReturn VisitTypedIndex(TypedIndex typedIndex, TArg arg);
		public abstract TReturn VisitTypedCast(TypedCast typedCast, TArg arg);
		public abstract TReturn VisitTypedBinary(TypedBinary typedBinary, TArg arg);
		public abstract TReturn VisitTypedUnary(TypedUnary typedUnary, TArg arg);
	}
	interface ITypedExpressionVisitorWithArg<TArg>
	{
		public abstract void VisitTypedIdentifier(TypedIdentifier typedIdentifier, TArg arg);
		public abstract void VisitTypedInitalizerList(TypedInitalizerList typedInitalizerList, TArg arg);
		public abstract void VisitTypedLiteral(TypedLiteral typedLiteral, TArg arg);
		public abstract void VisitTypedSizeOf(TypedSizeOf typedSizeOf, TArg arg);
		public abstract void VisitTypedCall(TypedCall typedCall, TArg arg);
		public abstract void VisitTypedIndex(TypedIndex typedIndex, TArg arg);
		public abstract void VisitTypedCast(TypedCast typedCast, TArg arg);
		public abstract void VisitTypedBinary(TypedBinary typedBinary, TArg arg);
		public abstract void VisitTypedUnary(TypedUnary typedUnary, TArg arg);
	}
}
