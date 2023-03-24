using System;
using System.Collections.Generic;
using Ripple.Utils;
using Ripple.Validation;
using Ripple.Validation.Info.Types;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Expressions;
using Ripple.Lexing;


namespace Ripple.Validation.Info.Types
{
	abstract class TypeInfo
	{

		public TypeInfo()
		{
		}

		public abstract void Accept(ITypeInfoVisitor iTypeInfoVisitor);
		public abstract T Accept<T>(ITypeInfoVisitor<T> iTypeInfoVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> iTypeInfoVisitor, TArg arg);
		public abstract void Accept<TArg>(ITypeInfoVisitorWithArg<TArg> iTypeInfoVisitor, TArg arg);
	}
}
