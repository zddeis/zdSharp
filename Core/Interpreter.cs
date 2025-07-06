using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;
using zds.Core.Statements;
using zds.Core.Exceptions;

namespace zds.Core
{
    public class Interpreter
    {
        private Environment _environment;

        public Interpreter(Environment _globals)
        {
            _environment = _globals;

            _globals = Natives.Initialize(_globals, this);
        }

        public void Run(List<IStatement> statements)
        {
            try
            {
                Execute(statements);
            }
            catch (RuntimeException ex)
            {
                Log.Error($"{ex.Message} (line {ex.Line})");
            }
            catch (ParseException ex)
            {
                Log.Error($"{ex.Message} (line {ex.Line})");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
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
            try
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
            catch (RuntimeException)
            {
                throw; // Re-throw runtime exceptions that already have line info
            }
            catch (Exception ex)
            {
                // For other exceptions, we don't have line info
                throw new RuntimeException(ex.Message, 0);
            }
        }

        private void ExecuteForStatement(ForStatement forStmt)
        {
            // Evaluate start, end, and step expressions
            var startValue = EvaluateExpression(forStmt.Start);
            var endValue = EvaluateExpression(forStmt.End);
            var stepValue = forStmt.Step != null ? EvaluateExpression(forStmt.Step) : 1.0;

            if (startValue is not double start)
                throw new RuntimeException("For loop start value must be a number", 0);
            if (endValue is not double end)
                throw new RuntimeException("For loop end value must be a number", 0);
            if (stepValue is not double step)
                throw new RuntimeException("For loop step value must be a number", 0);

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

        private object? EvaluateCall(CallExpression call)
        {
            try
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

                throw new RuntimeException($"'{call._name}' is not defined as a function", call.Line);
            }
            catch (RuntimeException)
            {
                throw; // Re-throw runtime exceptions that already have line info
            }
            catch (Exception ex)
            {
                throw new RuntimeException(ex.Message, call.Line);
            }
        }

        private object? EvaluateExpression(IExpression expression)
        {
            try
            {
                return expression switch
                {
                    AssignmentExpression assign => EvaluateAssignment(assign),
                    IndexAssignmentExpression indexAssign => indexAssign.Evaluate(),
                    PropertyExpression property => property.Evaluate(),
                    PropertyAccessExpression propertyAccess => propertyAccess.Evaluate(),
                    MethodCallExpression methodCall => methodCall.Evaluate(),
                    CallExpression call => EvaluateCall(call),
                    BinaryExpression binary => binary.Evaluate(),
                    LiteralExpression literal => literal.Evaluate(),
                    VariableExpression variable => _environment.Get(variable._name),
                    ArrayExpression array => array.Evaluate(),
                    IndexExpression index => index.Evaluate(),
                    _ => throw new RuntimeException($"Unknown expression type: {expression.GetType()}", 0)
                };
            }
            catch (RuntimeException)
            {
                throw; // Re-throw runtime exceptions that already have line info
            }
            catch (Exception ex)
            {
                int line = expression is IExpression expr ? expr.Line : 0;
                throw new RuntimeException(ex.Message, line);
            }
        }

        private object? EvaluateAssignment(AssignmentExpression assign)
        {
            try
            {
                var value = EvaluateExpression(assign._value);
                _environment.Define(assign._name, value);
                return value;
            }
            catch (RuntimeException)
            {
                throw; // Re-throw runtime exceptions that already have line info
            }
            catch (Exception ex)
            {
                throw new RuntimeException(ex.Message, assign.Line);
            }
        }

        private bool IsTruthy(object? value)
        {
            if (value == null) return false;
            if (value is bool b) return b;
            return true;
        }
    }
}