
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;

/*

C:\Users\david\Desktop\ZDSharp Projects\Program1.zds

*/

namespace zdSharp
{
    static class Program
    {
        static void Main(string[] args)
        {
            Global.FilePath = args.Length == 0 ? "" : args[0];

            Console.Title = "ZD#";
            Console.WriteLine(" Type 'help' or 'credits'");

            // Read File Lines
            while (Global.FilePath == "")
            {
                Global.FilePath = Commands.Run();
                Global.FilePath = File.Exists(Global.FilePath) ? Global.FilePath : "";
                Global.FilePath = Commands.VerifyExtension() ? Global.FilePath : "";
            }

            Console.Clear();
            Console.Title = Global.FileName();
            Global.FileLines = File.ReadLines(Global.FilePath).ToArray();


            // Lexer


            List<Token> tokens = new List<Token>();

            try
            {
                for (int i = 0; i < Global.FileLines.Length; i++)
                {
                    Global.CurrentLine = i + 1;
                    List<Token> TokenLine = new Lexer(Global.FileLines[i]).Tokenize();

                    for (int j = 0; j < TokenLine.Count; j++)
                    {
                        tokens.Add(TokenLine[j]);
                    }
                }

                if (Debug.Lexer)
                {
                    Log.Custom("DEBUG", "TOKENS:");

                    foreach (var token in tokens)
                        Console.WriteLine($"  {token.Type} : {token.Value}");

                    Console.WriteLine();
                    Console.ReadKey(true);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Lexer: {ex.Message} \n");
                Console.ReadKey(true);
                return;
            }


            // Parser


            var parser = new Parser(tokens);
            ASTNode ast = parser.Parse();

            try
            {
                if (Debug.Parser)
                {
                    Log.Custom("DEBUG", "AST:");
                    Console.WriteLine(ast + "\n");
                    Console.ReadKey(true);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Parser: {ex.Message} \n");
                Console.ReadKey(true);
                return;
            }


            // Interpreter


            var interpreter = new Interpreter();

            try
            {
                if (Debug.Intepreter)
                {
                    Log.Custom("DEBUG", "OUTPUT:");
                }

                interpreter.Interpret(ast);
            }
            catch (Exception ex)
            {
                Log.Error($"Runtime: {ex.Message}");
                Console.ReadKey(true);
                return;
            }
        }
    }
}

