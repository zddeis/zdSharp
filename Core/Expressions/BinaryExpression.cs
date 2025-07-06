using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Exceptions;

namespace zds.Core.Expressions
{
    public class BinaryExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly Token _operator;
        private readonly IExpression _right;
        public int Line => _operator.Line;

        public BinaryExpression(IExpression left, Token op, IExpression right)
        {
            _left = left;
            _operator = op;
            _right = right;
        }

        public object? Evaluate()
        {
            try
            {
                var left = _left.Evaluate();
                var right = _right.Evaluate();

                return _operator.Type switch
                {
                    TokenType.Plus => Add(left, right),
                    TokenType.Minus => Subtract(left, right),
                    TokenType.Multiply => Multiply(left, right),
                    TokenType.Divide => Divide(left, right),
                    TokenType.EqualsEquals => Equals(left, right),
                    TokenType.NotEquals => NotEquals(left, right),
                    TokenType.Or => Or(left, right),
                    TokenType.And => And(left, right),
                    TokenType.Greater => Greater(left, right),
                    TokenType.Less => Less(left, right),
                    TokenType.GreaterEquals => GreaterEquals(left, right),
                    TokenType.LessEquals => LessEquals(left, right),
                    _ => throw new RuntimeException($"Unknown operator {_operator.Type}", Line)
                };
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

        private static object Add(object? left, object? right)
        {
            if (left is double l && right is double r) return l + r;
            if (left is string || right is string) return $"{left}{right}";
            throw new Exception("Invalid operands for addition");
        }

        private static object Subtract(object? left, object? right)
        {
            if (left is double l && right is double r) return l - r;
            throw new Exception("Invalid operands for subtraction");
        }

        private static object Multiply(object? left, object? right)
        {
            if (left is double l && right is double r) return l * r;
            throw new Exception("Invalid operands for multiplication");
        }

        private static object Divide(object? left, object? right)
        {
            if (left is double l && right is double r)
            {
                if (r == 0) return 0;
                return l / r;
            }
            throw new Exception("Invalid operands for division");
        }

        private static new bool Equals(object? left, object? right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            return left.Equals(right);
        }

        private static bool NotEquals(object? left, object? right)
        {
            return !Equals(left, right);
        }

        private static bool Or(object? left, object? right)
        {
            if (left is bool l && right is bool r)
                return l || r;
            throw new Exception("Operands must be boolean values");
        }

        private static bool And(object? left, object? right)
        {
            if (left is bool l && right is bool r)
                return l && r;
            throw new Exception("Operands must be boolean values");
        }

        private static bool Greater(object? left, object? right)
        {
            if (left is double l && right is double r)
                return l > r;
            throw new Exception("Operands must be numbers");
        }

        private static bool Less(object? left, object? right)
        {
            if (left is double l && right is double r)
                return l < r;
            throw new Exception("Operands must be numbers");
        }

        private static bool GreaterEquals(object? left, object? right)
        {
            if (left is double l && right is double r)
                return l >= r;
            throw new Exception("Operands must be numbers");
        }

        private static bool LessEquals(object? left, object? right)
        {
            if (left is double l && right is double r)
                return l <= r;
            throw new Exception("Operands must be numbers");
        }
    }
}