using System;


namespace Ripple.AST
{
	abstract class TypeName
	{
		public abstract void Accept(ITypeNameVisitor iTypeNameVisitor);
		public abstract T Accept<T>(ITypeNameVisitor<T> iTypeNameVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> iTypeNameVisitor, TArg arg);
	}
}
