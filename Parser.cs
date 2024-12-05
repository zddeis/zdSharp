using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _current = 0;
        }

        public ASTNode Parse()
        {
            var statements = new List<ASTNode>();

            while (!IsAtEnd())
            {
                statements.Add(ParseStatement());
            }

            return new BlockNode(statements);
        }

        private ASTNode ParseStatement()
        {
            Token CurrentToken = Peek();
            NextPos();

            if (CurrentToken.Value == "set")
            {
                return ParseAssignment();
            }
            
            if (CurrentToken.Value == "print")
            {
                return ParsePrint();
            }

            if(CurrentToken.Value == "WaitKey")
            {
                return ParseWaitKey();
            }

            if(CurrentToken.Type == "String")
            {
                return new StringNode(CurrentToken.Value);
            }

            throw new Exception($"Unexpected token: {Peek().Value} : {Peek().Line}");
        }

        private ASTNode ParseAssignment()
        {
            var name = Consume("Identifier", "Expected variable name.");
            Consume("", "=", "Expected '='.");
            var value = ParseExpression();

            return new AssignmentNode(name.Value, value);
        }

        private ASTNode ParsePrint()
        {
            Consume("", "(", "Expected '(' after 'print'.");
            var expression = ParseExpression();
            Consume("", ")", "Expected ')' after 'print' expression.");

            return new PrintNode(new List<ASTNode> { expression });
        }

        private ASTNode ParseWaitKey()
        {
            Consume("", "(", "Expected '(' after 'WaitKey'.");
            Consume("", ")", "Expected ')' after 'WaitKey' expression.");

            return new WaitKeyNode(GetArguments());
        }

        private List<ASTNode> GetArguments()
        {
            return new List<ASTNode> { };
        }

        private ASTNode ParseExpression()
        {
            var node = ParseFactor();

            while (Match("Operator", "+") || Match("Operator", "-"))
            {
                var operatorToken = PrevPos();
                var right = ParseFactor();
                node = new BinaryExpressionNode(node, operatorToken.Value, right);
            }

            return node;
        }

        private ASTNode ParseFactor()
        {
            var node = ParsePrimary();

            while (Match("Operator", "*") || Match("Operator", "/"))
            {
                var operatorToken = PrevPos();
                var right = ParsePrimary();
                node = new BinaryExpressionNode(node, operatorToken.Value, right);
            }

            return node;
        }

        private ASTNode ParsePrimary()
        {
            if (Match("Number"))
            {
                try
                {
                    return new NumberNode(double.Parse(PrevPos().Value, CultureInfo.InvariantCulture));
                }
                catch (Exception)
                {
                    return new NumberNode(0);
                }
            }

            if (Match("Identifier"))
            {
                return new VariableNode(PrevPos().Value);
            }

            if (Match("Operator", "("))
            {
                var expression = ParseExpression();
                Consume("", ")", "Expected ')' after expression.");
                return expression;
            }

            if (Match("String"))
            {
                return new StringNode(PrevPos().Value);
            }

            throw new Exception($"Unexpected token: {Peek().Value} : {Peek().Line}");
        }

        private bool Match(string type, string value)
        {
            if (Check(type, value))
            {
                NextPos();
                return true;
            }
            return false;
        }

        private bool Match(string type)
        {
            if (Check(type))
            {
                NextPos();
                return true;
            }
            return false;
        }

        private bool Check(string type, string value)
        {
            if (IsAtEnd()) return false;
            var token = Peek();

            return type == "" ? token.Value == value : token.Type == type && token.Value == value;
        }

        private bool Check(string type)
        {
            if (IsAtEnd()) return false;
            var token = Peek();

            return token.Type == type;
        }

        private Token Consume(string type, string errorMessage)
        {
            if (Check(type))
                return NextPos();

            throw new Exception(errorMessage + $"\nInstead has type('{type.ToString()}') : {Peek().Line}");
        }

        private Token Consume(string type, string value, string errorMessage)
        {
            if (Check(type, value))
                return NextPos();

            throw new Exception(errorMessage + $"\nInstead has: type('{type.ToString()}'), value('{value.ToString()}') : {Peek().Line}");
        }

        private Token NextPos()
        {
            if (!IsAtEnd()) _current++;
            return PrevPos();
        }

        private Token Peek() => IsAtEnd() ? null : _tokens[_current];

        private Token PrevPos() => _tokens[_current - 1];

        private bool IsAtEnd() => _current >= _tokens.Count;
    }
}
