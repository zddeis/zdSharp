using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Exceptions
{
    class ParseException : Exception
    {
        public int Line { get; }

        public ParseException(string message, int line) : base(message)
        {
            Line = line;
        }
    }
}
