using System;


namespace ASTGeneration.Tests
{
	class Test : Expression
	{
		public readonly string a;
		public readonly string b;
		public readonly string c;
		public readonly string d;
		public readonly string e;
		public readonly string f;
		public readonly string g;
		public readonly string h;
		public readonly string i;

		public Test(string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			this.e = e;
			this.f = f;
			this.g = g;
			this.h = h;
			this.i = i;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitTest(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitTest(this);
		}

		public override bool Equals(object other)
		{
			if(other is Test test)
			{
				return a.Equals(test.a) && b.Equals(test.b) && c.Equals(test.c) && d.Equals(test.d) && e.Equals(test.e) && f.Equals(test.f) && g.Equals(test.g) && h.Equals(test.h) && i.Equals(test.i);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(a);
			code.Add(b);
			code.Add(c);
			code.Add(d);
			code.Add(e);
			code.Add(f);
			code.Add(g);
			code.Add(h);
			code.Add(i);
			return code.ToHashCode();
		}
	}
}
