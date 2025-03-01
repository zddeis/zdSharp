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
        private static readonly Random _random = new Random();

        public Interpreter(Environment _globals)
        {
            _environment = _globals;

            // Native Vars

            _globals.Define("pi", Math.PI);
            _globals.Define("tau", Math.Tau);
            _globals.Define("e", Math.E);
            _globals.Define("phi", (1 + Math.Sqrt(5)) / 2);
            _globals.Define("sqrt2", Math.Sqrt(2));
            _globals.Define("sqrt3", Math.Sqrt(3));

            _globals.Define("MaxValue", double.MaxValue);
            _globals.Define("MinValue", double.MinValue);
            _globals.Define("nan", double.NaN);

            _globals.Define("True", true);
            _globals.Define("False", false);

            _globals.Define("null", null);
            _globals.Define("nil", null);

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

            // Window Functions
            _globals.Define("window", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("window() requires two number arguments: sizeX and sizeY");

                if (args[0] is not double width)
                    throw new Exception("First argument (sizeX) must be a number");

                if (args[1] is not double height)
                    throw new Exception("Second argument (sizeY) must be a number");

                var window = new Window((int)width, (int)height);
                window.Show();
                return window;
            }));

            // Panel Functions
            _globals.Define("createPanel", new NativeFunction((args) =>
            {
                if (args.Count < 1)
                    throw new Exception("createPanel() requires a window argument");

                if (args[0] is not Window window)
                    throw new Exception("First argument must be a window object");

                int width = window.GetForm().ClientSize.Width;
                int height = window.GetForm().ClientSize.Height;

                return new Panel(window, width, height);
            }));

            // Timer Functions
            _globals.Define("setTimeout", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("setTimeout() requires a function and delay time in milliseconds");

                if (args[0] is not Function callback)
                    throw new Exception("First argument must be a function");

                if (args[1] is not double delayMs)
                    throw new Exception("Second argument must be a number (delay in milliseconds)");

                var timer = new System.Windows.Forms.Timer();
                timer.Interval = (int)delayMs;
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    try
                    {
                        callback.Call(this, new List<object?>());
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error in setTimeout callback: {ex.Message}");
                    }
                };
                timer.Start();

                return timer;
            }));

            _globals.Define("setInterval", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("setInterval() requires a function and interval time in milliseconds");

                if (args[0] is not Function callback)
                    throw new Exception("First argument must be a function");

                if (args[1] is not double intervalMs)
                    throw new Exception("Second argument must be a number (interval in milliseconds)");

                var timer = new System.Windows.Forms.Timer();
                timer.Interval = (int)intervalMs;
                timer.Tick += (sender, e) =>
                {
                    try
                    {
                        callback.Call(this, new List<object?>());
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error in setInterval callback: {ex.Message}");
                        timer.Stop();
                    }
                };
                timer.Start();

                return timer;
            }));

            _globals.Define("clearTimeout", new NativeFunction((args) =>
            {
                if (args.Count < 1)
                    throw new Exception("clearTimeout() requires a timer object");

                if (args[0] is System.Windows.Forms.Timer timer)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                return null;
            }));

            // Key handling functions
            _globals.Define("onKeyDown", new NativeFunction((args) =>
            {
                if (args.Count < 3)
                    throw new Exception("onKeyDown() requires three arguments: window, key, and handler function");

                if (args[0] is not Window window)
                    throw new Exception("First argument must be a window object");

                string key = args[1]?.ToString() ?? "";
                if (string.IsNullOrEmpty(key))
                    throw new Exception("Second argument (key) must be a non-empty string");

                if (args[2] is not Function handler)
                    throw new Exception("Third argument must be a function");

                window.OnKeyDown(key, handler);
                return null;
            }));

            _globals.Define("onKeyUp", new NativeFunction((args) =>
            {
                if (args.Count < 3)
                    throw new Exception("onKeyUp() requires three arguments: window, key, and handler function");

                if (args[0] is not Window window)
                    throw new Exception("First argument must be a window object");

                string key = args[1]?.ToString() ?? "";
                if (string.IsNullOrEmpty(key))
                    throw new Exception("Second argument (key) must be a non-empty string");

                if (args[2] is not Function handler)
                    throw new Exception("Third argument must be a function");

                window.OnKeyUp(key, handler);
                return null;
            }));

            // Math Functions

            _globals.Define("max", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("max() requires two number arguments");

                if (args[0] is not double n1)
                    throw new Exception("First argument must be a number");

                if (args[1] is not double n2)
                    throw new Exception("Second argument must be a number");

                return Math.Max(n1, n2);
            }));

            _globals.Define("min", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("min() requires two number arguments");

                if (args[0] is not double n1)
                    throw new Exception("First argument must be a number");

                if (args[1] is not double n2)
                    throw new Exception("Second argument must be a number");

                return Math.Min(n1, n2);
            }));

            _globals.Define("clamp", new NativeFunction((args) =>
            {
                if (args.Count < 3)
                    throw new Exception("clamp() requires three number arguments: min, n, max");

                if (args[0] is not double minVal)
                    throw new Exception("First argument (min) must be a number");

                if (args[1] is not double n)
                    throw new Exception("Second argument (n) must be a number");

                if (args[2] is not double maxVal)
                    throw new Exception("Third argument (max) must be a number");

                // Implement clamp logic
                if (n < minVal) return minVal;
                if (n > maxVal) return maxVal;
                return n;
            }));

            _globals.Define("random", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("random() requires at least 2 arguments: min and max");

                if (args[0] is not double minVal)
                    throw new Exception("First argument (min) must be a number");

                if (args[1] is not double maxVal)
                    throw new Exception("Second argument (max) must be a number");

                if (minVal > maxVal)
                    throw new Exception("Min value cannot be greater than max value");

                // Check if decimals parameter is provided
                if (args.Count > 2)
                {
                    if (args[2] is not double decimalPlaces)
                        throw new Exception("Third argument (decimals) must be a number");

                    int decimals = (int)decimalPlaces;
                    if (decimals < 0)
                        throw new Exception("Decimal places cannot be negative");

                    // Generate random with specified decimal places
                    double randomValue = minVal + (_random.NextDouble() * (maxVal - minVal));
                    return Math.Round(randomValue, decimals);
                }

                // Integer random (no decimals)
                return (double)_random.Next((int)minVal, (int)maxVal + 1);
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

            _globals.Define("sin", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                if (args[0] is not double d) return 0;
                return Math.Sin(d);
            }));

            _globals.Define("cos", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                if (args[0] is not double d) return 0;
                return Math.Cos(d);
            }));

            _globals.Define("tan", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                if (args[0] is not double d) return 0;
                return Math.Tan(d);
            }));

            _globals.Define("sqrt", new NativeFunction((args) =>
            {
                if (args.Count == 0) return 0;
                if (args[0] is not double d) return 0;
                if (d < 0) throw new Exception("Cannot calculate square root of a negative number");
                return Math.Sqrt(d);
            }));

            _globals.Define("pow", new NativeFunction((args) =>
            {
                if (args.Count < 2) return 0;
                if (args[0] is not double baseNum) return 0;
                if (args[1] is not double exponent) return 0;
                return Math.Pow(baseNum, exponent);
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
                    case "List`1":
                        return (double)((List<object>)args[0]).Count;
                    case "null":
                        return 0;
                    default:
                        return (double)(args[0].ToString()).Length;
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
                    case "Window":
                        return "window";
                    default:
                        return type.ToLower();
                }
            }));

            // Array Operations
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

            _globals.Define("sort", new NativeFunction((args) =>
            {
                if (args.Count < 1)
                    throw new Exception("sort() requires an array argument");

                if (args[0] == null)
                    throw new Exception("Cannot sort a null array");

                if (args[0] is not List<object?> array)
                    throw new Exception("First argument must be an array");

                // Create a new array to avoid modifying the original
                var sortedArray = new List<object?>(array);

                // Sort the array (only works reliably for arrays of the same type)
                sortedArray.Sort((a, b) =>
                {
                    if (a == null && b == null) return 0;
                    if (a == null) return -1;
                    if (b == null) return 1;

                    if (a is double numA && b is double numB)
                        return numA.CompareTo(numB);

                    if (a is string strA && b is string strB)
                        return strA.CompareTo(strB);

                    return a.ToString().CompareTo(b.ToString());
                });

                return sortedArray;
            }));

            _globals.Define("map", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("map() requires at least 2 arguments: array and function");

                if (args[0] == null)
                    throw new Exception("Cannot map a null array");

                if (args[0] is not List<object?> array)
                    throw new Exception("First argument must be an array");

                if (args[1] is not Function mapFunction)
                    throw new Exception("Second argument must be a function");

                var result = new List<object?>();

                foreach (var item in array)
                {
                    var itemArgs = new List<object?> { item };
                    result.Add(mapFunction.Call(this, itemArgs));
                }

                return result;
            }));

            _globals.Define("filter", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("filter() requires at least 2 arguments: array and function");

                if (args[0] == null)
                    throw new Exception("Cannot filter a null array");

                if (args[0] is not List<object?> array)
                    throw new Exception("First argument must be an array");

                if (args[1] is not Function filterFunction)
                    throw new Exception("Second argument must be a function");

                var result = new List<object?>();

                foreach (var item in array)
                {
                    var itemArgs = new List<object?> { item };
                    var shouldInclude = filterFunction.Call(this, itemArgs);

                    if (shouldInclude is bool include && include)
                    {
                        result.Add(item);
                    }
                }

                return result;
            }));

            _globals.Define("find", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("find() requires at least 2 arguments: array and function");

                if (args[0] == null)
                    return null;

                if (args[0] is not List<object?> array)
                    throw new Exception("First argument must be an array");

                if (args[1] is not Function findFunction)
                    throw new Exception("Second argument must be a function");

                foreach (var item in array)
                {
                    var itemArgs = new List<object?> { item };
                    var isMatch = findFunction.Call(this, itemArgs);

                    if (isMatch is bool match && match)
                    {
                        return item;
                    }
                }

                return null;
            }));

            // String Manipulation
            _globals.Define("substring", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("substring() requires at least 2 arguments: string and start index");

                if (args[0] == null)
                    return "";

                if (args[0] is not string str)
                    throw new Exception("First argument must be a string");

                if (args[1] is not double startDouble)
                    throw new Exception("Second argument must be a number");

                int start = (int)startDouble;

                if (start < 0 || start >= str.Length)
                    throw new Exception($"Start index {start} out of bounds for string of length {str.Length}");

                if (args.Count > 2)
                {
                    if (args[2] is not double lengthDouble)
                        throw new Exception("Third argument must be a number");

                    int length = (int)lengthDouble;

                    if (length < 0)
                        throw new Exception("Length cannot be negative");

                    // Ensure we don't go past the end of the string
                    length = Math.Min(length, str.Length - start);

                    return str.Substring(start, length);
                }

                return str.Substring(start);
            }));

            _globals.Define("replace", new NativeFunction((args) =>
            {
                if (args.Count < 3)
                    throw new Exception("replace() requires 3 arguments: string, old value, and new value");

                if (args[0] == null)
                    return "";

                if (args[0] is not string str)
                    throw new Exception("First argument must be a string");

                string oldValue = args[1]?.ToString() ?? "";
                string newValue = args[2]?.ToString() ?? "";

                return str.Replace(oldValue, newValue);
            }));

            _globals.Define("split", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("split() requires at least 2 arguments: string and delimiter");

                if (args[0] == null)
                    return new List<object?>();

                if (args[0] is not string str)
                    throw new Exception("First argument must be a string");

                string delimiter = args[1]?.ToString() ?? "";

                if (string.IsNullOrEmpty(delimiter))
                    throw new Exception("Delimiter cannot be empty");

                string[] parts = str.Split(delimiter);

                // Convert string array to List<object?>
                var result = new List<object?>();
                foreach (var part in parts)
                {
                    result.Add(part);
                }

                return result;
            }));

            _globals.Define("join", new NativeFunction((args) =>
            {
                if (args.Count < 2)
                    throw new Exception("join() requires at least 2 arguments: array and delimiter");

                if (args[0] == null)
                    return "";

                if (args[0] is not List<object?> array)
                    throw new Exception("First argument must be an array");

                string delimiter = args[1]?.ToString() ?? "";

                // Convert each element to string and join
                var stringParts = array.Select(item => item?.ToString() ?? "null");
                return string.Join(delimiter, stringParts);
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

        public object? ExecuteBlock(List<IStatement> statements, Dictionary<string, object?>? passingParams = null)
        {
            // Save previous values
            Dictionary<string, object?> previousValues = new(_environment._values);
            object? lastValue = null;

            try
            {
                // Define passed parameters
                if (passingParams != null)
                    foreach (var param in passingParams)
                        _environment.Define(param.Key, param.Value);

                // Execute the block
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
                // Restore previous values
                foreach (var param in _environment._values)
                    if (!previousValues.TryGetValue(param.Key, out object? value))
                        _environment._values.Remove(param.Key);
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

            // Save the current value of the loop variable
            object? previousValue = _environment._values.TryGetValue(forStmt.Variable, out object? value) ? value : null;

            // Initialize the loop variable
            _environment.Define(forStmt.Variable, start);

            // Loop direction
            bool ascending = step > 0;

            // Execute the loop
            while ((ascending && (double)_environment.Get(forStmt.Variable) <= end) ||
                  (!ascending && (double)_environment.Get(forStmt.Variable) >= end))
            {
                Execute(forStmt.Body);

                // Update the loop variable
                double currentValue = (double)_environment.Get(forStmt.Variable);
                _environment.Define(forStmt.Variable, currentValue + step);
            }

            // Restore the previous value of the loop variable
            if (previousValue == null)
                _environment._values.Remove(forStmt.Variable);
            else
                _environment.Define(forStmt.Variable, previousValue);
        }

        private string? FormatValue(object? value)
        {
            if (value == null)
                return "null";

            string type = value.GetType().Name;

            // Format Arrays
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

            // Format Functions
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

            // Format Windows
            if (type == "Window")
            {
                return "[Window Object]";
            }

            // Return other types as strings
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
                PropertyExpression property => property.Evaluate(),
                CallExpression call => EvaluateCall(call),
                BinaryExpression binary => binary.Evaluate(),
                LiteralExpression literal => literal.Evaluate(),
                VariableExpression variable => _environment.Get(variable._name),
                ArrayExpression array => array.Evaluate(),
                IndexExpression index => index.Evaluate(),
                _ => throw new Exception($"Unknown expression type: {expression.GetType()}")
            };
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