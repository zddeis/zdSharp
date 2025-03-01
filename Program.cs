using System;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using zds.Core;
using zds.Core.Statements;
using System.Windows.Forms;

namespace zds
{
    internal class Program
    {
        public static bool Debug = false;
        public static string Version = "v0.1";
        public static Interpreter CurrentInterpreter { get; private set; }

        [STAThread]
        static void Main(string[] args)
        {
            // Initialize Windows Forms but don't hide the console
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Make sure the console window is visible
            Console.Title = "ZD#" + " - " + Version;
            Log.Write(" Type 'help' or 'credits'");

            string FilePath = args.Length > 0 ? args[0] : "";

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

                var parser = new Parser(tokens, _globals);
                var statements = parser.Parse();

                var interpreter = new Interpreter(_globals);
                CurrentInterpreter = interpreter;
                interpreter.Run(statements);

                // Keep the application running until all windows are closed
                if (Application.OpenForms.Count > 0)
                {
                    Application.Run();
                }
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