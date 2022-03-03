using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    static class TypeChecker
    {
        public static bool CheckAST(AbstractSyntaxTree ast, out string message)
        {
            ASTTypeCheckerVisitor visitor = new ASTTypeCheckerVisitor();

            ASTInfoBuilder builder = new ASTInfoBuilder();
            builder.PushPrimaryTypes();
            ASTInfo info = builder.BuildInfo();

            return visitor.CheckAST(ast, info, out message);
        }
    }
}
