using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class MemberDeclarationStmt : Statement 
    {
        public readonly List<Token> Attributes;
        public readonly Statement Member;

        public MemberDeclarationStmt(List<Token> attributes, Statement member)
        {
            Attributes = attributes;
            Member = member;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitMemberDeclaration(this);
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitMemberDeclaration(this);
        }
    }
}
