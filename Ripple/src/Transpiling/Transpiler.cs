using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;

namespace Ripple.Transpiling
{
    static class Transpiler
    {
        public static TranspilerResult Transpile(FileStmt file, string fileName)
        {
            TranspilerVisitor visitor = new TranspilerVisitor(fileName);
            string code = visitor.Transpile(file);
            string[] src = code.Split(CStatement.Package.Seperator);
            return new TranspilerResult(fileName + ".h", src[0], fileName + ".c", src[1]);
        }
    }
}
