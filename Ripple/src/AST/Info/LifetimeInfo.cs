using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class LifetimeInfo
    {
        public static readonly LifetimeInfo Static = new LifetimeInfo(new Token("static", TokenType.Lifetime, -1, -1));

        private readonly Either<int, Token> m_LifetimeValue;

        public LifetimeInfo(Token lifetime)
        {
            if (lifetime.Type != TokenType.Lifetime)
                throw new ArgumentException("Token must be a lifetime.");

            m_LifetimeValue = new Either<int, Token>(lifetime);
        }

        public LifetimeInfo(int lifetime)
        {
            m_LifetimeValue = new Either<int, Token>(lifetime);
        }

        public Token? LifetimeToken => m_LifetimeValue.IsOptionB ? m_LifetimeValue.BValue : null;

        public bool IsAssignableTo(LifetimeInfo other)
        {
            return other.m_LifetimeValue.Match(lifetimeLength =>
            {
                return m_LifetimeValue.Match(
                    self => self <= lifetimeLength,
                    self => false);
            },
            liftimeToken =>
            {
                return m_LifetimeValue.Match(
                    self => false,
                    self => self.Equals(liftimeToken));
            });
        }

        public override bool Equals(object obj)
        {
            return obj is LifetimeInfo info &&
                   EqualityComparer<Either<int, Token>>.Default.Equals(m_LifetimeValue, info.m_LifetimeValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_LifetimeValue);
        }

        public void Match(Action<int> local, Action<Token> parameter)
		{
            m_LifetimeValue.Match(local, parameter);
		}

        public TReturn Match<TReturn>(Func<int, TReturn> local, Func<Token, TReturn> parameter)
		{
            return m_LifetimeValue.Match(local, parameter);
		}

        public override string ToString()
        {
            return m_LifetimeValue.Match(i =>
            {
                return  "'" + i.ToString();
            },
            t =>
            {
                return t.Text;
            });
        }
    }
}
