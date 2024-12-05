using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    class Debug
    {
        public static bool Lexer = true;
        public static bool Parser = true;
        public static bool Intepreter = Lexer || Parser;
    }
    class Global
    {
        public static string Extension = "zds";
        public static string Docs = "docs.html";

        public static string[] FileLines;
        public static string FilePath = "";

        public static int CurrentLine = 0;

        public static bool Error = false;

        public static string FileName()
        {
            string FileName = "";

            for (int i = FilePath.Length - 1; i >= 0; i--)
            {
                if (FilePath[i] == '\\') { return FileName; }
                FileName = FilePath[i] + FileName;
            }

            return FileName;
        }

        public static double EvaluateExpression(string expression)
        {
            try
            {
                DataTable Table = new DataTable();
                object Result = Table.Compute(expression, string.Empty);

                return Convert.ToDouble(Result);
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        public static string ValueToString(dynamic value)
        {
            return value.ToString();
        }
    }
}

