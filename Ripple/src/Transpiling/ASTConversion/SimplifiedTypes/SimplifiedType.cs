using System;


namespace Ripple.Transpiling.ASTConversion.SimplifiedTypes
{
	abstract class SimplifiedType
	{
		public abstract void Accept(ISimplifiedTypeVisitor iSimplifiedTypeVisitor);
		public abstract T Accept<T>(ISimplifiedTypeVisitor<T> iSimplifiedTypeVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ISimplifiedTypeVisitor<TReturn, TArg> iSimplifiedTypeVisitor, TArg arg);
		public abstract void Accept<TArg>(ISimplifiedTypeVisitorWithArg<TArg> iSimplifiedTypeVisitor, TArg arg);
	}
}
