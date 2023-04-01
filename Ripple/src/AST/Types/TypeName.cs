using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public abstract class TypeName
	{

		public TypeName()
		{
		}

		public abstract void Accept(ITypeNameVisitor iTypeNameVisitor);
		public abstract T Accept<T>(ITypeNameVisitor<T> iTypeNameVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypeNameVisitor<TReturn, TArg> iTypeNameVisitor, TArg arg);
		public abstract void Accept<TArg>(ITypeNameVisitorWithArg<TArg> iTypeNameVisitor, TArg arg);
	}
}
