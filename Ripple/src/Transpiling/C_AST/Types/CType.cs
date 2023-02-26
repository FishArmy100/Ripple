using System;


namespace Ripple.Transpiling.C_AST
{
	abstract class CType
	{
		public abstract void Accept(ICTypeVisitor iCTypeVisitor);
		public abstract T Accept<T>(ICTypeVisitor<T> iCTypeVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> iCTypeVisitor, TArg arg);
	}
}
