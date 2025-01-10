using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class VariableExpression : IExpression
    {
        public readonly string _name;
        private readonly Environment _environment;

        public VariableExpression(string name, Environment environment)
        {
            _name = name;
            _environment = environment;
        }

        public object? Evaluate()
        {
            return _environment.Get(_name);
        }
    }
}