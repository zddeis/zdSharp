using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;
using zds.Core.Statements;

namespace zds.Core
{
    public class Interpreter
    {
        private Environment _environment;

        public Interpreter(Environment _globals)
        {
            _environment = _globals;

            // Native Vars

            _globals.Define("pi", Math.PI);

            // Native Functions

            _globals.Define("waitKey", new NativeFunction((args) =>
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                string result;

                result = key.Key switch
                {
                    ConsoleKey.D1 => "1",
                    ConsoleKey.D2 => "2",
                    ConsoleKey.D3 => "3",
                    ConsoleKey.D4 => "4",
                    ConsoleKey.D5 => "5",
                    ConsoleKey.D6 => "6",
                    ConsoleKey.D7 => "7",
                    ConsoleKey.D8 => "8",
                    ConsoleKey.D9 => "9",
                    ConsoleKey.D0 => "0",
                    _ => key.Key.ToString()
                };

                return result;
            }));

            _globals.Define("round", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                return Math.Round(Convert.ToDecimal(args[0]));
            }));

            _globals.Define("floor", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                return Math.Floor(Convert.ToDecimal(args[0]));
            }));

            _globals.Define("abs", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                return Math.Abs(Convert.ToDecimal(args[0]));
            }));

            _globals.Define("clear", new NativeFunction((args) =>
            {
                Console.Clear();
                return null;
            }));

            _globals.Define("epoch", new NativeFunction((args) =>
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                return (double)t.TotalSeconds;
            }));

            _globals.Define("print", new NativeFunction((args) =>
            {
                string result = "";
                foreach (var arg in args)
                {
                    result += FormatValue(arg);
                }

                Console.WriteLine(result);
                return result;
            }));

            _globals.Define("input", new NativeFunction((args) =>
            {
                string prompt = args.Count > 0 ? args[0].ToString() : "";
                Console.Write(prompt);
                return Console.ReadLine();
            }));

            _globals.Define("length", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;

                string type = args[0]?.GetType().Name ?? "null";

                switch (type)
                {
                    case "String":
                        return (double)((string)args[0]).Length;
                    case "List`1":
                        return (double)((List<object>)args[0]).Count;
                    case "null":
                        return 0;
                    default:
                        return 0;
                }
            }));

            _globals.Define("typeOf", new NativeFunction((args) =>
            {
                if (args.Count == 0 || args[0] == null) return "null";

                string type = args[0].GetType().Name;

                switch (type)
                {
                    case "List`1":
                        return "array";
                    case "Double":
                        return "number";
                    default:
                        return type.ToLower();
                }
            }));

            _globals.Define("insert", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("insert() requires at least 2 arguments: array and element");

                if (args[0] == null)
                    throw new Exception("Cannot insert into a null array");

                if (args[0] is not List<object?> array)
                    throw new Exception("First argument must be an array");

                // Add the new element to the array
                array.Add(args[1]);

                // Return the modified array
                return array;
            }));
        }

        public void Run(List<IStatement> statements)
        {
            Execute(statements);
        }

        private void Execute(List<IStatement> statements)
        {
            foreach (var statement in statements)
                ExecuteStatement(statement);
        }

        public object? ExecuteBlock(List<IStatement> statements, Environment environment)
        {
            var previous = _environment;
            object? lastValue = null;

            try
            {
                _environment = environment;

                foreach (var statement in statements)
                {
                    if (statement is ReturnStatement ret)
                    {
                        object? value = ret.Value != null ? EvaluateExpression(ret.Value) : null;
                        throw new ReturnException(value);
                    }
                    else if (statement is ExpressionStatement expr)
                    {
                        lastValue = EvaluateExpression(expr.Expression);
                    }
                    else
                    {
                        ExecuteStatement(statement);
                    }
                }
                return lastValue;
            }
            finally
            {
                _environment = previous;
            }
        }

        private void ExecuteStatement(IStatement statement)
        {
            switch (statement)
            {
                case ExpressionStatement expr:
                    EvaluateExpression(expr.Expression);
                    break;
                case FunctionStatement func:
                    var function = new Function(func, _environment);
                    _environment.Define(func.Name, function);
                    break;
                case IfStatement ifStmt:
                    if (IsTruthy(EvaluateExpression(ifStmt.Condition)))
                        Execute(ifStmt.ThenBranch);
                    else if (ifStmt.ElseBranch != null)
                        Execute(ifStmt.ElseBranch);
                    break;
                case ReturnStatement ret:
                    object? value = ret.Value != null ? EvaluateExpression(ret.Value) : null;
                    throw new ReturnException(value);
                case WhileStatement whileStmt:
                    while (IsTruthy(EvaluateExpression(whileStmt.Condition)))
                        Execute(whileStmt.Body);
                    break;
                case ForStatement forStmt:
                    ExecuteForStatement(forStmt);
                    break;
                default:
                    throw new Exception($"Unknown statement type: {statement.GetType()}");
            }
        }

        private void ExecuteForStatement(ForStatement forStmt)
        {
            // Evaluate start, end, and step expressions
            var startValue = EvaluateExpression(forStmt.Start);
            var endValue = EvaluateExpression(forStmt.End);
            var stepValue = forStmt.Step != null ? EvaluateExpression(forStmt.Step) : 1.0;

            if (startValue is not double start)
                throw new Exception("For loop start value must be a number");
            if (endValue is not double end)
                throw new Exception("For loop end value must be a number");
            if (stepValue is not double step)
                throw new Exception("For loop step value must be a number");

            // Initialize the loop variable
            _environment.Define(forStmt.Variable, start);

            // Determine loop direction
            bool ascending = step > 0;

            // Execute the loop
            while ((ascending && (double)_environment.Get(forStmt.Variable) <= end) ||
                  (!ascending && (double)_environment.Get(forStmt.Variable) >= end))
            {
                // Execute the body
                Execute(forStmt.Body);

                // Update the loop variable
                double currentValue = (double)_environment.Get(forStmt.Variable);
                _environment.Define(forStmt.Variable, currentValue + step);
            }
        }

        private string? FormatValue(object? value)
        {
            if (value == null)
                return "null";

            string type = value.GetType().Name;

            if (type == "List`1")
            {
                string result = "[";
                List<object> list = (List<object>)value;

                for (int i = 0; i < list.Count; i++)
                {
                    result += FormatValue(list[i]);
                    if (i < list.Count - 1)
                        result += ", ";
                }
                result += "]";
                return result;
            }

            if (type == "Function" && value is Core.Function function)
            {
                string result = function._declaration.Name + "(";

                for (int i = 0; i < function._declaration.Parameters.Count; i++)
                {
                    result += function._declaration.Parameters[i];
                    if (i < function._declaration.Parameters.Count - 1)
                        result += ", ";
                }

                result += ")";
                return result;
            }

            return value.ToString();
        }

        private object? EvaluateCall(CallExpression call)
        {
            var callee = _environment.Get(call._name);

            if (callee is NativeFunction nativeFunction)
            {
                var arguments = call._arguments.Select(arg => EvaluateExpression(arg)).ToList();
                return nativeFunction.Call(arguments);
            }

            if (callee is Function function)
            {
                var arguments = call._arguments.Select(arg => EvaluateExpression(arg)).ToList();

                return function.Call(this, arguments);
            }

            throw new Exception($"'{call._name}' is not defined as a function");
        }

        private object? EvaluateExpression(IExpression expression)
        {
            return expression switch
            {
                AssignmentExpression assign => EvaluateAssignment(assign),
                IndexAssignmentExpression indexAssign => indexAssign.Evaluate(),
                CallExpression call => EvaluateCall(call),
                BinaryExpression binary => binary.Evaluate(),
                LiteralExpression literal => literal.Evaluate(),
                VariableExpression variable => _environment.Get(variable._name),
                ArrayExpression array => array.Evaluate(),
                IndexExpression index => EvaluateIndexExpression(index),
                _ => throw new Exception($"Unknown expression type: {expression.GetType()}")
            };
        }

        private object? EvaluateIndexExpression(IndexExpression index)
        {
            return index.Evaluate();
        }

        private object? EvaluateAssignment(AssignmentExpression assign)
        {
            var value = EvaluateExpression(assign._value);
            _environment.Define(assign._name, value);
            return value;
        }

        private bool IsTruthy(object? value)
        {
            if (value == null) return false;
            if (value is bool b) return b;
            return true;
        }
    }

    public class NativeFunction
    {
        private readonly Func<List<object?>, object?> _function;

        public NativeFunction(Func<List<object?>, object?> function)
        {
            _function = function;
        }

        public object? Call(List<object?> arguments)
        {
            return _function(arguments);
        }
    }
}