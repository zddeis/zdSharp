using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zds.Core.Statements;

namespace zds
{
    class Log
    {
        private static void Reset()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Reset();
        }

        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning: " + message);
            Reset();
        }

        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Info: " + message);
            Reset();
        }

        public static void Write(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Reset();
        }

        public static void Custom(string type, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(type + ": " + message);
            Reset();
        }
    }
}
