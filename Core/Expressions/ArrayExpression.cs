using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class ArrayExpression : IExpression
    {
        private readonly List<IExpression> _elements;

        public ArrayExpression(List<IExpression> elements)
        {
            _elements = elements;
        }

        public object? Evaluate()
        {
            return _elements.Select(e => e.Evaluate()).ToList();
        }
    }
}