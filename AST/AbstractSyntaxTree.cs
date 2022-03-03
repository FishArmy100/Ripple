using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class AbstractSyntaxTree
    {
        private readonly Expression m_Expression;

        public AbstractSyntaxTree(Expression expr)
        {
            m_Expression = expr;
        }

        public void Accept(IASTVisitor visitor)
        {
            m_Expression.Accept(visitor);
        }

        public T Accept<T>(IASTVisitor<T> visitor)
        {
            return m_Expression.Accept(visitor);
        }
    }
}
