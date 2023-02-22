using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.IO;
using Ripple.Validation.Info;
using Ripple.Lexing;
using Ripple.AST.Utils;

namespace Ripple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            App.Application app = new App.Application();
            app.Run();
        }
    }
}
