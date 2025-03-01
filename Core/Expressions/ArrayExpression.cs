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
        public int Line { get; }

        public ArrayExpression(List<IExpression> elements, int line = 0)
        {
            _elements = elements;
            Line = line;
        }

        public object? Evaluate()
        {
            try
            {
                return _elements.Select(e => e.Evaluate()).ToList();
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

    public class IndexExpression : IExpression
    {
        public readonly IExpression _array;
        public readonly IExpression _index;
        public int Line { get; }

        public IndexExpression(IExpression array, IExpression index, int line = 0)
        {
            _array = array;
            _index = index;
            Line = line;
        }

        public object? Evaluate()
        {
            try
            {
                var array = _array.Evaluate();
                var index = _index.Evaluate();

                if (array is not List<object?> list)
                    throw new RuntimeException("Cannot index a non-array value", Line);

                if (index is not double d)
                    throw new RuntimeException("Array index must be a number", Line);

                int i = (int)d;

                if (i < 0 || i >= list.Count)
                    throw new RuntimeException($"Array index {i} out of bounds", Line);

                return list[i];
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

    public class IndexAssignmentExpression : IExpression
    {
        private readonly IExpression _array;
        private readonly IExpression _index;
        private readonly IExpression _value;
        public int Line { get; }

        public IndexAssignmentExpression(IExpression array, IExpression index, IExpression value, int line = 0)
        {
            _array = array;
            _index = index;
            _value = value;
            Line = line;
        }

        public object? Evaluate()
        {
            try
            {
                var array = _array.Evaluate();
                var index = _index.Evaluate();
                var value = _value.Evaluate();

                if (array is not List<object?> list)
                    throw new RuntimeException("Cannot index a non-array value", Line);

                if (index is not double d)
                    throw new RuntimeException("Array index must be a number", Line);

                int i = (int)d;

                if (i < 0 || i >= list.Count)
                    throw new RuntimeException($"Array index {i} out of bounds", Line);

                list[i] = value;
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