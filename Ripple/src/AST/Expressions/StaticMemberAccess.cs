using System.Collections.Generic;
using Ripple.Lexing;
using Ripple.Parsing;
using Raucse;
using System;
using System.Linq;


namespace Ripple.AST
{
	public class StaticMemberAccess : Expression
	{
		public readonly TypeName Type;
		public readonly Token ColonColon;
		public readonly Token MemberName;

		public StaticMemberAccess(TypeName type, Token colonColon, Token memberName)
		{
			this.Type = type;
			this.ColonColon = colonColon;
			this.MemberName = memberName;
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.VisitStaticMemberAccess(this);
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitStaticMemberAccess(this);
		}

		public override TReturn Accept<TReturn, TArg>(IExpressionVisitor<TReturn, TArg> visitor, TArg arg)
		{
			return visitor.VisitStaticMemberAccess(this, arg);
		}

		public override void Accept<TArg>(IExpressionVisitorWithArg<TArg> visitor, TArg arg)
		{
			visitor.VisitStaticMemberAccess(this, arg);
		}

		public override bool Equals(object other)
		{
			if(other is StaticMemberAccess staticMemberAccess)
			{
				return Type.Equals(staticMemberAccess.Type) && ColonColon.Equals(staticMemberAccess.ColonColon) && MemberName.Equals(staticMemberAccess.MemberName);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode code = new HashCode();
			code.Add(Type);
			code.Add(ColonColon);
			code.Add(MemberName);
			return code.ToHashCode();
		}
	}
}
