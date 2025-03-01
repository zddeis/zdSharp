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
        public int Line { get; }

        public VariableExpression(string name, Environment environment, int line = 0)
        {
            _name = name;
            _environment = environment;
            Line = line;
        }

        public object? Evaluate()
        {
            try
            {
                return _environment.Get(_name);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(ex.Message, Line);
            }
        }
    }
}