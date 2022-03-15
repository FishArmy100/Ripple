using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class BreakStmt : Statement
    {
        public readonly Token BreakToken;

        public BreakStmt(Token breakToken)
        {
            BreakToken = breakToken;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitBreakStmt(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBreakStmt(this);
        }
    }
}
