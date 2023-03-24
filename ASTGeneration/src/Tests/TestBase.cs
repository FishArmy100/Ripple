using System;
using System.Collections.Generic;


namespace ASTGeneration.Tests
{
	abstract class TestBase
	{

		public TestBase()
		{
		}

		public abstract void Accept(ITestBaseVisitor iTestBaseVisitor);
		public abstract T Accept<T>(ITestBaseVisitor<T> iTestBaseVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITestBaseVisitor<TReturn, TArg> iTestBaseVisitor, TArg arg);
		public abstract void Accept<TArg>(ITestBaseVisitorWithArg<TArg> iTestBaseVisitor, TArg arg);
	}
}
