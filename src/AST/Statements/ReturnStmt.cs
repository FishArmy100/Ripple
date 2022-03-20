using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class ReturnStmt : Statement
    {
        public readonly Token ReturnToken;
        public readonly Expression ReturnExpression;

        public ReturnStmt(Token returnToken, Expression returnExpression)
        {
            ReturnToken = returnToken;
            ReturnExpression = returnExpression;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitReturnStmt(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }
}
