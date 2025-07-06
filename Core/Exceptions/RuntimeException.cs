using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Exceptions
{
    class RuntimeException : Exception
    {
        public int Line { get; }

        public RuntimeException(string message, int line) : base(message)
        {
            Line = line;
        }
    }
}
