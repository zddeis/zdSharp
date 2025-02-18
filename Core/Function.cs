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

            // Validate argument count
            if (arguments.Count != _declaration.Parameters.Count)
            {
                throw new Exception($"Expected {_declaration.Parameters.Count} arguments but got {arguments.Count}");
            }

            // Define parameters in the function's environment
            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                environment.Define(_declaration.Parameters[i], arguments[i]);
            }

            try
            {
                return interpreter.ExecuteBlock(_declaration.Body, environment);
                
            }
            catch (ReturnException returnValue)
            {
                return returnValue.Value;
            }
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