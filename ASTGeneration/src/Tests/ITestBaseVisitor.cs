using System;


namespace ASTGeneration.Tests
{
	interface ITestBaseVisitor
	{
		public abstract void VisitTest1(Test1 test1);
		public abstract void VisitTest2(Test2 test2);
	}

	interface ITestBaseVisitor<T>
	{
		public abstract T VisitTest1(Test1 test1);
		public abstract T VisitTest2(Test2 test2);
	}

	interface ITestBaseVisitor<TReturn, TArg>
	{
		public abstract TReturn VisitTest1(Test1 test1, TArg arg);
		public abstract TReturn VisitTest2(Test2 test2, TArg arg);
	}
	interface ITestBaseVisitorWithArg<TArg>
	{
		public abstract void VisitTest1(Test1 test1, TArg arg);
		public abstract void VisitTest2(Test2 test2, TArg arg);
	}
}
