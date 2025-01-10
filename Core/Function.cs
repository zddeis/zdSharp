using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zds.Core.Statements;

namespace zds.Core
{
    public class Function
    {
        private readonly FunctionStatement _declaration;
        private readonly Environment _closure;

        public Function(FunctionStatement declaration, Environment closure)
        {
            _declaration = declaration;
            _closure = closure;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            var environment = new Environment(_closure);

            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                environment.Define(_declaration.Parameters[i], arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (ReturnException returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }
    }

    public class ReturnException : Exception
    {
        public object? Value { get; }

        public ReturnException(object? value)
        {
            Value = value;
        }
    }
}