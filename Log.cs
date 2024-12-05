using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    class Log
    {
        public static void WriteMessage(string message, ConsoleColor Color)
        {
            ConsoleColor _ConsoleColor = Console.ForegroundColor;

            Console.ForegroundColor = Color;
            Console.WriteLine(message);
            Console.ForegroundColor = _ConsoleColor;
        }

        public static void Info(dynamic message) { WriteMessage($"INFO - {message.ToString()}", ConsoleColor.Cyan); }
        public static void Warn(dynamic message) { WriteMessage($"WARN - {message.ToString()}", ConsoleColor.Yellow); }
        public static void Error(dynamic message) { WriteMessage($"ERROR - {message.ToString()}", ConsoleColor.Red); }
        public static void Custom(string type, dynamic message) { WriteMessage($"{type} - {message}", ConsoleColor.Green); }

        public static void Error_Expected(string Expected, dynamic Value, int Line)
        {
            Global.Error = true;

            Error($"{Expected} expected, instead has: '{Value.ToString()}' ; Line: {Line}.");
        }

        public static void Error_ExpectedAfter(string Expected, string After, int Line)
        {
            Global.Error = true;

            Error($"{Expected} expected after '{After}'; Line: {Line}.");
        }
    }
}

