using System;
using System.Collections.Generic;
using Ripple.Utils;
using System.Linq;


namespace Ripple.Transpiling.C_AST
{
	abstract class CType
	{

		public CType()
		{
		}

		public abstract void Accept(ICTypeVisitor iCTypeVisitor);
		public abstract T Accept<T>(ICTypeVisitor<T> iCTypeVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ICTypeVisitor<TReturn, TArg> iCTypeVisitor, TArg arg);
		public abstract void Accept<TArg>(ICTypeVisitorWithArg<TArg> iCTypeVisitor, TArg arg);
	}
}
