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

    public class IndexExpression : IExpression
    {
        public readonly IExpression _array;
        public readonly IExpression _index;

        public IndexExpression(IExpression array, IExpression index)
        {
            _array = array;
            _index = index;
        }

        public object? Evaluate()
        {
            var array = _array.Evaluate();
            var index = _index.Evaluate();

            if (array is not List<object?> list)
                throw new Exception("Cannot index a non-array value");

            if (index is not double d)
                throw new Exception("Array index must be a number");

            int i = (int)d;

            if (i < 0 || i >= list.Count)
                throw new Exception($"Array index {i} out of bounds");

            return list[i];
        }
    }

    public class IndexAssignmentExpression : IExpression
    {
        private readonly IExpression _array;
        private readonly IExpression _index;
        private readonly IExpression _value;

        public IndexAssignmentExpression(IExpression array, IExpression index, IExpression value)
        {
            _array = array;
            _index = index;
            _value = value;
        }

        public object? Evaluate()
        {
            var array = _array.Evaluate();
            var index = _index.Evaluate();
            var value = _value.Evaluate();

            if (array is not List<object?> list)
                throw new Exception("Cannot index a non-array value");

            if (index is not double d)
                throw new Exception("Array index must be a number");

            int i = (int)d;

            if (i < 0 || i >= list.Count)
                throw new Exception($"Array index {i} out of bounds");

            list[i] = value;
            return value;
        }
    }
}