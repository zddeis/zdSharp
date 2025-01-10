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

        public CallExpression(string name, List<IExpression> arguments)
        {
            _name = name;
            _arguments = arguments;
        }

        public object? Evaluate()
        {
            throw new NotImplementedException();
        }
    }
}