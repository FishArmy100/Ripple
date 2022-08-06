using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class ForLoopStmt : Statement
    {
        public readonly Statement Initializer;
        public readonly Expression Condition;
        public readonly Expression Incrementer;
        public readonly Statement Body;

        public ForLoopStmt(Statement initializer, Expression condition, Expression incrementer, Statement body)
        {
            Initializer = initializer;
            Condition = condition;
            Incrementer = incrementer;
            Body = body;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitForLoop(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitForLoop(this);
        }
    }
}
