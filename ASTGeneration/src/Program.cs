using System;
using System.Collections.Generic;

namespace ASTGeneration
{
    struct Token { }

    class Program
    {
        static void Main(string[] args)
        {
            AstGenerator.Generate("C:\\dev\\Ripple\\ASTGeneration\\src\\Tests", "AST", "Expression", new List<string>()
            {
                "Term : Token Left, Token Op, Token Right"
            });
        }
    }
}
