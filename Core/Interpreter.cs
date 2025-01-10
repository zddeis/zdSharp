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

            _globals.Assign("pi", Math.PI);

            // Native Functions

            _globals.Define("waitKey", new NativeFunction((args) =>
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                string result;

                result = key.Key switch
                {
                    ConsoleKey.D1 => "1", ConsoleKey.D2 => "2", ConsoleKey.D3 => "3",
                    ConsoleKey.D4 => "4", ConsoleKey.D5 => "5", ConsoleKey.D6 => "6",
                    ConsoleKey.D7 => "7", ConsoleKey.D8 => "8", ConsoleKey.D9 => "9",
                    ConsoleKey.D0 => "0", _ => key.Key.ToString()
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

            _globals.Define("print", new NativeFunction((args) =>
            {
                // Joins every string from args with White-Space separating
                string result = string.Join(" ", args.Select(arg => arg?.ToString()));

                Console.WriteLine(result);
                return result;
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

        public void ExecuteBlock(List<IStatement> statements, Environment environment)
        {
            var previous = _environment;

            try
            {
                _environment = environment;
                Execute(statements);
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
                default:
                    throw new Exception($"Unknown statement type: {statement.GetType()}");
            }
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
                CallExpression call => EvaluateCall(call),
                BinaryExpression binary => binary.Evaluate(),
                LiteralExpression literal => literal.Evaluate(),
                VariableExpression variable => _environment.Get(variable._name),
                ArrayExpression array => array.Evaluate(),
                _ => throw new Exception($"Unknown expression type: {expression.GetType()}")
            };
        }

        private object? EvaluateAssignment(AssignmentExpression assign)
        {
            var value = EvaluateExpression(assign._value);
            _environment.Assign(assign._name, value);
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