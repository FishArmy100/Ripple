using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class WhileLoopStmt : Statement
    {
        public readonly Expression Condition;
        public readonly Statement Body;

        public WhileLoopStmt(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitWhileLoop(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitWhileLoop(this);
        }
    }
}
