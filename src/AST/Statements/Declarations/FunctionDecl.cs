using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class FunctionDecl : Statement
    {
        public readonly Token ReturnType;
        public readonly Token Name;
        public readonly List<FunctionParameter> Parameters;
        public readonly BlockStmt Body;

        public FunctionDecl(Token returnType, Token name, List<FunctionParameter> parameters, BlockStmt body)
        {
            ReturnType = returnType;
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitFuncDeclaration(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitFuncDeclaration(this);
        }
    }
}
