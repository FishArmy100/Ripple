using System;
using System.Collections.Generic;


namespace ASTGeneration.Tests
{
	class Test2 : TestBase
	{
		public readonly int Z;
		public readonly int W;

		public Test2(int z, int w)
		{
			this.Z = z;
			this.W = w;
		}

		public override void Accept(ITestBaseVisitor visitor)
		{
			visitor.VisitTest2(this);
		}

		public override T Accept<T>(ITestBaseVisitor<T> visitor)
		{
			return visitor.VisitTest2(this);
		}

		public override TReturn Accept<TReturn, TArg>(ITestBaseVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitTest2(this, arg);
		}

		public override void Accept<TArg>(ITestBaseVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitTest2(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is Test2 test2)
			{
				return Z.Equals(test2.Z) && W.Equals(test2.W);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Z);
			code.Add(W);
			return code.ToHashCode();
		}
	}
}
