using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class CallExpression : IExpression
    {
        public string _name { get; }
        public List<IExpression> _arguments { get; }
        public int Line { get; }

        public CallExpression(string name, List<IExpression> arguments, int line = 0)
        {
            _name = name;
            _arguments = arguments;
            Line = line;
        }

        public object? Evaluate()
        {
            throw new RuntimeException("Not implemented", Line);
        }
    }
}