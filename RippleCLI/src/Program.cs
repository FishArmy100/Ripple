using System;


namespace RippleCLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Application app = new Application();
            app.Run();
        }
    }
}
