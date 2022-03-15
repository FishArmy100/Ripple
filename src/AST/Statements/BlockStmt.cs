using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class BlockStmt : Statement
    {
        public readonly List<Statement> Statements;

        public BlockStmt(List<Statement> statements)
        {
            Statements = statements;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitBlock(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}
