using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    public class Interpreter : IVisitor
    {
        private readonly Dictionary<string, dynamic> _variables = new();

        public void Interpret(ASTNode node)
        {
            node.Accept(this);
        }

        public void Interpret(ASTNode node, Dictionary<string, dynamic> passing_variables)
        {
            foreach (var kvp in passing_variables)
            {
                _variables[kvp.Key] = kvp.Value;
            }

            node.Accept(this);
        }

        public void Visit(BlockNode node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
            }
        }

        public void Visit(AssignmentNode node)
        {
            _variables[node.VariableName] = Evaluate(node.Value);
        }

        public void Visit(PrintNode node)
        {
            Console.WriteLine(Evaluate(node.Expression));
        }
        public void Visit(WaitKeyNode node)
        {
            node.Key = Console.ReadKey(true).KeyChar.ToString();
        }

        public void Visit(BinaryExpressionNode node)
        {
            throw new InvalidOperationException("BinaryExpressionNode must be evaluated via Evaluate.");
        }

        public void Visit(VariableNode node)
        {
            throw new InvalidOperationException("VariableNode must be evaluated via Evaluate.");
        }

        public void Visit(NumberNode node)
        {
            throw new InvalidOperationException("NumberNode must be evaluated via Evaluate.");
        }

        public void Visit(StringNode node)
        {
            throw new InvalidOperationException("StringNode must be evaluated via Evaluate.");
        }

        private dynamic Evaluate(ASTNode node)
        {
            switch (node)
            {
                case StringNode stringNode:
                    return stringNode.Value;

                case NumberNode numberNode:
                    return numberNode.Value;

                case VariableNode variableNode:
                    if (_variables.TryGetValue(variableNode.Name, out var value))
                        return value;
                    throw new Exception($"Undefined variable: {variableNode.Name}");

                case BinaryExpressionNode binaryNode:
                    return EvaluateBinary(binaryNode);

                default:
                    throw new Exception($"Unknown ASTNode type: {node.GetType().Name}");
            }

            /*
            return node switch
            {
                NumberNode numberNode => numberNode.Value,
                VariableNode variableNode => _variables.TryGetValue(variableNode.Name, out var value)
                    ? value
                    : throw new Exception($"Undefined variable: {variableNode.Name}"),
                BinaryExpressionNode binaryNode => EvaluateBinary(binaryNode),
                _ => throw new Exception($"Unknown ASTNode type: {node.GetType().Name}")
            };
            */

        }

        private double EvaluateBinary(BinaryExpressionNode node)
        {
            var left = Evaluate(node.Left);
            var right = Evaluate(node.Right);

            return node.Operator switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right != 0 ? left / right : throw new Exception("Division by zero"),
                _ => throw new Exception($"Unknown operator: {node.Operator}")
            };
        }
    }
}
