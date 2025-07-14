using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds
{
    class Commands
    {
        private static string Extension = "zds";
        private static string Docs = "https://zddeis.github.io/zdSharp_docs/";

        private static string help = "" +
                        "\n 1. Write program file path or open it with ZD#" +
                        "\n " +
                        "\n 2. Other commands:" +
                        "\n " +
                        "\n  help    - List of commands               " +
                        "\n  quit    - Close this window              " +
                        "\n  q       - Close this window              " +
                        "\n  clear   - Clear this window              " +
                        "\n  docs    - Opens ZD# documentation        ";

        public static bool VerifyExtension(string FilePath)
        {
            int Len_FP = FilePath.Length;
            int Len_EX = Extension.Length;

            for (int i = 0; i < Math.Min(Len_EX, Len_FP); i++)
            {
                int j = Len_FP - Len_EX + i;

                if (FilePath[j] == Extension[i]) { continue; }

                return false;
            }

            return true;
        }

        public static string Run()
        {
            Console.Write("\n <ZD#> ");

            string? Command = Console.ReadLine();

            switch (Command)
            {
                case "help":

                    Console.WriteLine(help);
                    break;

                case "exit":
                case "quit":
                case "q":

                    Environment.Exit(0);
                    break;

                case "clear":

                    Console.Clear();
                    break;

                case "docs":

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Docs,
                        UseShellExecute = true // Required to open URLs in the default browser
                    });
                    break;

                default:

                    if (File.Exists(Command))
                    {
                        return Command;
                    }

                    Console.WriteLine("\n '" + Command + "'");
                    Console.WriteLine(" Command not recognized");
                    break;
            }

            return "";
        }
    }
}

