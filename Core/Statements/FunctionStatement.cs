using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;

namespace zds.Core.Statements
{
    public class FunctionStatement : IStatement
    {
        public string Name { get; }
        public List<string> Parameters { get; }
        public List<IStatement> Body { get; }

        public FunctionStatement(string name, List<string> parameters, List<IStatement> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }
    }
}