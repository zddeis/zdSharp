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

        public LiteralExpression(object? value)
        {
            _value = value;
        }

        public object? Evaluate() => _value;
    }
}