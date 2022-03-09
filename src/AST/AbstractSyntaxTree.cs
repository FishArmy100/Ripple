using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class AbstractSyntaxTree
    {
        private readonly List<Statement> m_Statments;

        public AbstractSyntaxTree(List<Statement> statements)
        {
            m_Statments = statements;
        }

        public void Accept(IASTVisitor visitor)
        {
            foreach (Statement statement in m_Statments)
                statement.Accept(visitor);
        }

        public List<T> Accept<T>(IASTVisitor<T> visitor)
        {
            List<T> values = new List<T>();

            foreach (Statement statement in m_Statments)
                values.Add(statement.Accept(visitor));

            return values;
        }
    }
}
