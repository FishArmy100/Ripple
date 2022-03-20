using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class IfStmt : Statement
    {
        public readonly Expression Condition;
        public readonly Statement ThenBranch;
        public readonly Statement ElseBranch;

        public IfStmt(Expression condition, Statement thenBranch, Statement elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitIfStmt(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }
}
