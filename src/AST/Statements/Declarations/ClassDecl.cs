using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class ClassDecl : Statement
    {
        public readonly Token Name;
        public readonly Token? Base;
        public readonly List<MemberDeclarationStmt> MemberDeclarations;

        public ClassDecl(Token name, Token? baseName, List<MemberDeclarationStmt> memberDeclarations)
        {
            Name = name;
            Base = baseName;
            MemberDeclarations = memberDeclarations;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitClassDeclaration(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitClassDeclaration(this);
        }

        public bool IsDerived => Base != null;
    }
}
