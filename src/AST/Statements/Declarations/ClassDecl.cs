using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.AST
{
    class ClassDecl : Statement
    {
        public readonly Token Name;
        public readonly Token? Base;
        public readonly List<MemberDeclarationStmt> MemberDeclarations;
        public readonly bool IsStatic;

        public ClassDecl(Token name, Token? baseName, List<MemberDeclarationStmt> memberDeclarations, bool isStatic)
        {
            Name = name;
            Base = baseName;
            MemberDeclarations = memberDeclarations;
            IsStatic = isStatic;
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
