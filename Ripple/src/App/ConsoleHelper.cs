using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.App
{
    static class ConsoleHelper
    {
        public const ConsoleColor DefaultColor = ConsoleColor.Black;
        public const ConsoleColor ErrorColor = ConsoleColor.Red;

        public static void WriteLineError(string error)
        {
            Console.BackgroundColor = ErrorColor;
            Console.WriteLine(error);
            Console.BackgroundColor = DefaultColor;
        }

        public static void WriteLine(string text)
		{
            Console.WriteLine(text);
		}
    }
}
