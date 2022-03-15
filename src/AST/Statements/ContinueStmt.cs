using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class ContinueStmt : Statement
    {
        public readonly Token ContinueToken;

        public ContinueStmt(Token continueToken)
        {
            ContinueToken = continueToken;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitContinueStmt(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitContinueStmt(this);
        }
    }
}
