using System;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using zds.Core;
using zds.Core.Statements;

namespace zds
{
    internal class Program
    {
        public static bool Debug = false;
        public static string Version = "v0.1";

        static void Main(string[] args)
        {
            string FilePath = args.Length > 0 ? args[0] : "";

            Console.Title = "ZD#" + " - " + Version;
            Log.Write(" Type 'help' or 'credits'");

            // Read File Lines

            while (FilePath == "")
            {
                FilePath = Commands.Run();
                FilePath = File.Exists(FilePath) ? FilePath : "";
                FilePath = Commands.VerifyExtension(FilePath) ? FilePath : "";
            }

            Console.Clear();
            Console.Title = FileName(FilePath);

            // Run Program
            
            try
            {
                Core.Environment _globals = new Core.Environment();
                string source = File.ReadAllText(FilePath);

                var tokenizer = new Tokenizer(source);
                var tokens = tokenizer.Tokenize();

                if(Debug)
                {
                    foreach(var token in tokens)
                    {
                        Log.Custom(token.Line.ToString(), token.Type.ToString() + " - " + token.Value.ToString());
                    }

                    Console.ReadKey();
                    Console.Clear();
                }

                var parser = new Parser(tokens, _globals);
                var statements = parser.Parse();

                var interpreter = new Interpreter(_globals);
                interpreter.Run(statements);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Console.ReadKey();
            }
        }

        static string FileName(string FilePath)
        {
            string FileName = "";

            for (int i = FilePath.Length - 1; i >= 0; i--)
            {
                if (FilePath[i] == '\\') { return FileName; }
                FileName = FilePath[i] + FileName;
            }

            return FileName;
        }
    }
}