using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Compiling.CCompilation
{
    class CCompilationError : CompilerError
    {
        public CCompilationError() : base(new SourceLocation())
        {

        }

        public override string GetMessage() => "Clang could not compile transpiled c code, which, as you can imagen, is not good.";
    }
}
