using System;


namespace Ripple.Validation.Info.Types
{
	abstract class TypeInfo
	{
		public abstract void Accept(ITypeInfoVisitor iTypeInfoVisitor);
		public abstract T Accept<T>(ITypeInfoVisitor<T> iTypeInfoVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> iTypeInfoVisitor, TArg arg);
		public abstract void Accept<TArg>(ITypeInfoVisitorWithArg<TArg> iTypeInfoVisitor, TArg arg);
	}
}
