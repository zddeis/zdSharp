using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class AssignmentExpression : IExpression
    {
        public string _name;
        public IExpression _value;
        public Environment _environment;

        public AssignmentExpression(string name, IExpression value, Environment environment)
        {
            _name = name;
            _value = value;
            _environment = environment;
        }

        public object? Evaluate()
        {
            var value = _value.Evaluate();
            _environment.Assign(_name, value);
            return value;
        }
    }
}