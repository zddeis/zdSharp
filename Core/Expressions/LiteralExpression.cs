using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class LiteralExpression : IExpression
    {
        private readonly object? _value;
        public int Line { get; }

        public LiteralExpression(object? value, int line = 0)
        {
            _value = value;
            Line = line;
        }

        public object? Evaluate() => _value;
    }
}