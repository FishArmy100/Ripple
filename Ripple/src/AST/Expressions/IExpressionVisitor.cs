

namespace Ripple.AST
{
	public interface IExpressionVisitor
	{
		public abstract void VisitLiteral(Literal literal);
		public abstract void VisitGrouping(Grouping grouping);
		public abstract void VisitCall(Call call);
		public abstract void VisitIndex(Index index);
		public abstract void VisitCast(Cast cast);
		public abstract void VisitUnary(Unary unary);
		public abstract void VisitBinary(Binary binary);
		public abstract void VisitIdentifier(Identifier identifier);
		public abstract void VisitInitializerList(InitializerList initializerList);
		public abstract void VisitMemberAccess(MemberAccess memberAccess);
		public abstract void VisitSizeOf(SizeOf sizeOf);
	}

	public interface IExpressionVisitor<T>
	{
		public abstract T VisitLiteral(Literal literal);
		public abstract T VisitGrouping(Grouping grouping);
		public abstract T VisitCall(Call call);
		public abstract T VisitIndex(Index index);
		public abstract T VisitCast(Cast cast);
		public abstract T VisitUnary(Unary unary);
		public abstract T VisitBinary(Binary binary);
		public abstract T VisitIdentifier(Identifier identifier);
		public abstract T VisitInitializerList(InitializerList initializerList);
		public abstract T VisitMemberAccess(MemberAccess memberAccess);
		public abstract T VisitSizeOf(SizeOf sizeOf);
	}

	public interface IExpressionVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitLiteral(Literal literal, TArg arg);
		public abstract TReturn VisitGrouping(Grouping grouping, TArg arg);
		public abstract TReturn VisitCall(Call call, TArg arg);
		public abstract TReturn VisitIndex(Index index, TArg arg);
		public abstract TReturn VisitCast(Cast cast, TArg arg);
		public abstract TReturn VisitUnary(Unary unary, TArg arg);
		public abstract TReturn VisitBinary(Binary binary, TArg arg);
		public abstract TReturn VisitIdentifier(Identifier identifier, TArg arg);
		public abstract TReturn VisitInitializerList(InitializerList initializerList, TArg arg);
		public abstract TReturn VisitMemberAccess(MemberAccess memberAccess, TArg arg);
		public abstract TReturn VisitSizeOf(SizeOf sizeOf, TArg arg);
	}
	public interface IExpressionVisitorWithArg<TArg>
	{
		public abstract void VisitLiteral(Literal literal, TArg arg);
		public abstract void VisitGrouping(Grouping grouping, TArg arg);
		public abstract void VisitCall(Call call, TArg arg);
		public abstract void VisitIndex(Index index, TArg arg);
		public abstract void VisitCast(Cast cast, TArg arg);
		public abstract void VisitUnary(Unary unary, TArg arg);
		public abstract void VisitBinary(Binary binary, TArg arg);
		public abstract void VisitIdentifier(Identifier identifier, TArg arg);
		public abstract void VisitInitializerList(InitializerList initializerList, TArg arg);
		public abstract void VisitMemberAccess(MemberAccess memberAccess, TArg arg);
		public abstract void VisitSizeOf(SizeOf sizeOf, TArg arg);
	}
}
