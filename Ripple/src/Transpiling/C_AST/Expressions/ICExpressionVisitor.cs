using System;


namespace Ripple.Transpiling.C_AST
{
	interface ICExpressionVisitor
	{
		public abstract void VisitBinary(Binary binary);
		public abstract void VisitUnary(Unary unary);
		public abstract void VisitIndex(Index index);
		public abstract void VisitCall(Call call);
		public abstract void VisitCast(Cast cast);
		public abstract void VisitIdentifier(Identifier identifier);
		public abstract void VisitSizeOf(SizeOf sizeOf);
	}

	interface ICExpressionVisitor<T>
	{
		public abstract T VisitBinary(Binary binary);
		public abstract T VisitUnary(Unary unary);
		public abstract T VisitIndex(Index index);
		public abstract T VisitCall(Call call);
		public abstract T VisitCast(Cast cast);
		public abstract T VisitIdentifier(Identifier identifier);
		public abstract T VisitSizeOf(SizeOf sizeOf);
	}

	interface ICExpressionVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitBinary(Binary binary, TArg arg);
		public abstract TReturn VisitUnary(Unary unary, TArg arg);
		public abstract TReturn VisitIndex(Index index, TArg arg);
		public abstract TReturn VisitCall(Call call, TArg arg);
		public abstract TReturn VisitCast(Cast cast, TArg arg);
		public abstract TReturn VisitIdentifier(Identifier identifier, TArg arg);
		public abstract TReturn VisitSizeOf(SizeOf sizeOf, TArg arg);
	}
}
