using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core
{
    public class Environment
    {
        public Dictionary<string, object?> _values = new();
        private readonly Environment? _enclosing;

        public Environment(Environment? enclosing = null)
        {
            _enclosing = enclosing;
        }

        public object? Get(string name)
        {
            if (_values.ContainsKey(name))
                return _values[name];

            if (_enclosing != null)
                return _enclosing.Get(name);

            throw new Exception($"Undefined variable '{name}'");
        }

        public void Define(string name, object? value)
        {
            _values[name] = value;
        }
        public void Define(string[] names, object? value)
        {
            foreach (var name in names)
            {
                _values[name] = value;
            }
        }
    }
}