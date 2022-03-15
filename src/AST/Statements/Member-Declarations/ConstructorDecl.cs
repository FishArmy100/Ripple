using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class ConstructorDecl : Statement
    {
        public readonly Token ClassName;
        public readonly List<FunctionParameter> Parameters;
        public readonly BlockStmt Body;

        public ConstructorDecl(Token className, List<FunctionParameter> parameters, BlockStmt body)
        {
            ClassName = className;
            Parameters = parameters;
            Body = body;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitConstructorDecl(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitConstructorDecl(this);
        }
    }
}
