using System;
using System.Collections.Generic;


namespace ASTGeneration.Tests
{
	class Test1 : TestBase
	{
		public readonly int X;
		public readonly int Y;

		public Test1(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override void Accept(ITestBaseVisitor visitor)
		{
			visitor.VisitTest1(this);
		}

		public override T Accept<T>(ITestBaseVisitor<T> visitor)
		{
			return visitor.VisitTest1(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITestBaseVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTest1(this, arg);
		}

		public override void Accept<TArg>(ITestBaseVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTest1(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Test1 test1)
			{
				return X.Equals(test1.X) && Y.Equals(test1.Y);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(X);
			code.Add(Y);
			return code.ToHashCode();
		}
	}
}
