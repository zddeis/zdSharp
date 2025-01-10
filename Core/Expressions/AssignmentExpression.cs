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

        public AssignmentExpression(string name, IExpression value)
        {
            _name = name;
            _value = value;
        }

        public object? Evaluate()
        {
            throw new NotImplementedException();
        }
    }
}
