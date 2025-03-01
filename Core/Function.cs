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
        public readonly FunctionStatement _declaration;
        private readonly Environment _closure;

        public Function(FunctionStatement declaration, Environment closure)
        {
            _declaration = declaration;
            _closure = closure;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            // Validate argument count
            if (arguments.Count != _declaration.Parameters.Count)
            {
                throw new Exception($"Expected {_declaration.Parameters.Count} arguments but got {arguments.Count}");
            }

            Dictionary<string, object?>? passingParams = new();           

            // Add the arguments to the dictionary
            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                passingParams.Add(_declaration.Parameters[i], arguments[i]);
            }

            // If no arguments are passed, set the dictionary to null
            if (passingParams.Count == 0)
                passingParams = null;

            // Execute the function's body with the new vars
            try
            {
                return interpreter.ExecuteBlock(_declaration.Body, passingParams);
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