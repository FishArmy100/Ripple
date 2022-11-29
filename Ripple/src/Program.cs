using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.IO;
using Ripple.AST.Info;
using Ripple.Lexing;

namespace Ripple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            App.Application app = new App.Application();
            //app.Run();

            TypeInfo typeInfo = new TypeInfo.Basic(false, new PrimaryTypeInfo(new Token("int", TokenType.IntagerLiteral, 0, 0)));
            OperatorInfo.Unary unary = new OperatorInfo.Unary(typeInfo, TokenType.Minus, typeInfo);

            OperatorLibrary library = new OperatorLibrary();
            Console.WriteLine(library.UnaryOperators.TryAdd(unary));
            Console.WriteLine(library.UnaryOperators.Contains(TokenType.Minus, typeInfo));
        }
    }
}
