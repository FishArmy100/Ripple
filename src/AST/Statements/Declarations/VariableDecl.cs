using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class VariableDecl : Statement
    {
        public readonly Token TypeName;
        public readonly Token Name;
        public readonly Expression Initializer;

        public VariableDecl(Token typeName, Token name, Expression initializer)
        {
            TypeName = typeName;
            Name = name;
            Initializer = initializer;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitVarDeclaration(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitVarDeclaration(this);
        }
    }
}
