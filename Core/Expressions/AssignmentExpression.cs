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
        public int Line { get; }

        public AssignmentExpression(string name, IExpression value, Environment environment, int line = 0)
        {
            _name = name;
            _value = value;
            _environment = environment;
            Line = line;
        }

        public object? Evaluate()
        {
            try
            {
                var value = _value.Evaluate();
                _environment.Define(_name, value);
                return value;
            }
            catch (RuntimeException)
            {
                throw; // Re-throw runtime exceptions that already have line info
            }
            catch (Exception ex)
            {
                throw new RuntimeException(ex.Message, Line);
            }
        }
    }
}