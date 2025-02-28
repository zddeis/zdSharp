using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Expressions;
using zds.Core.Statements;

namespace zds.Core
{
    public class Parser
    {
        private readonly Environment _environment;
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens, Environment environment)
        {
            _environment = environment;
            _tokens = tokens;
            _current = 0;
        }

        private FunctionStatement FunctionDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected function name").Value.ToString()!;
            Consume(TokenType.LeftParen, "Expected '(' after function name");

            var parameters = new List<string>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    parameters.Add(Consume(TokenType.Identifier, "Expected parameter name").Value.ToString()!);
                } while (Match(TokenType.Comma));
            }

            Consume(TokenType.RightParen, "Expected ')' after parameters");
            var body = Block();

            return new FunctionStatement(name, parameters, body);
        }

        private ForStatement ForStatement()
        {
            var variable = Consume(TokenType.Identifier, "Expected variable name after 'for'").Value.ToString()!;
            Consume(TokenType.Equals, "Expected '=' after variable name in for loop");
            var start = Expression();

            Consume(TokenType.To, "Expected 'to' after start value in for loop");
            var end = Expression();

            IExpression? step = null;
            if (Match(TokenType.Step))
            {
                step = Expression();
            }

            Consume(TokenType.Then, "Expected 'then' after for loop declaration");
            var body = Block();

            return new ForStatement(variable, start, end, step, body);
        }

        private WhileStatement WhileStatement()
        {
            var condition = Expression();
            Consume(TokenType.Then, "Expected 'then' after while condition");
            var body = Block();

            return new WhileStatement(condition, body);
        }

        private IfStatement IfStatement()
        {
            var condition = Expression();
            Consume(TokenType.Then, "Expected 'then' after if condition");
            var thenBranch = Block();

            List<IStatement>? elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = Block();
            }

            return new IfStatement(condition, thenBranch, elseBranch);
        }

        private ReturnStatement ReturnStatement()
        {
            IExpression? value = null;
            if (!Check(TokenType.End))
                value = Expression();

            return new ReturnStatement(value);
        }

        private List<IStatement> Block()
        {
            var statements = new List<IStatement>();

            while (!Check(TokenType.End) && !Check(TokenType.Else) && !IsAtEnd())
                statements.Add(Statement());

            if (!Check(TokenType.Else))
            {
                Consume(TokenType.End, "Expected 'end' after block");
            }

            return statements;
        }

        public List<IStatement> Parse()
        {
            var statements = new List<IStatement>();

            while (!IsAtEnd())
                statements.Add(Statement());

            return statements;
        }

        private IStatement Statement()
        {
            if (Match(TokenType.Function)) return FunctionDeclaration();
            if (Match(TokenType.While)) return WhileStatement();
            if (Match(TokenType.If)) return IfStatement();
            if (Match(TokenType.Return)) return ReturnStatement();
            if (Match(TokenType.For)) return ForStatement();

            return new ExpressionStatement(Expression());
        }

        private IExpression Expression()
        {
            return Assignment();
        }

        private IExpression Assignment()
        {
            // Check for array index assignment: array[index] = value
            if (Match(TokenType.Identifier))
            {
                string name = Previous().Value.ToString()!;

                // Check if this is an array index assignment
                if (Match(TokenType.LeftBracket))
                {
                    var index = Expression();
                    Consume(TokenType.RightBracket, "Expected ']' after array index");

                    if (Match(TokenType.Equals))
                    {
                        var value = Expression();
                        var array = new VariableExpression(name, _environment);
                        return new IndexAssignmentExpression(array, index, value);
                    }

                    // If it's not an assignment, it's just an array access
                    _current -= 2; // Go back to before the left bracket
                }

                // Check for regular variable assignment
                if (Match(TokenType.Equals))
                {
                    var value = Expression();
                    return new AssignmentExpression(name, value, _environment);
                }

                _current--; // Go back to the identifier token
            }

            return LogicalOr();
        }

        private IExpression ParseOperators(IExpression left)
        {
            while (Match(TokenType.Or) || Match(TokenType.And) ||
                   Match(TokenType.Greater) || Match(TokenType.Less) ||
                   Match(TokenType.GreaterEquals) || Match(TokenType.LessEquals) ||
                   Match(TokenType.EqualsEquals) || Match(TokenType.NotEquals))
            {
                var op = Previous();
                var right = LogicalOr();
                left = new Expressions.BinaryExpression(left, op, right);
            }
            return left;
        }

        private IExpression LogicalOr()
        {
            var expr = LogicalAnd();

            while (Match(TokenType.Or))
            {
                var op = Previous();
                var right = LogicalAnd();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression LogicalAnd()
        {
            var expr = Equality();

            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.EqualsEquals) || Match(TokenType.NotEquals))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression Comparison()
        {
            var expr = Term();

            while (Match(TokenType.Greater) || Match(TokenType.Less) ||
                   Match(TokenType.GreaterEquals) || Match(TokenType.LessEquals))
            {
                var op = Previous();
                var right = Term();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression Term()
        {
            var expr = Factor();

            while (Match(TokenType.Plus) || Match(TokenType.Minus))
            {
                var op = Previous();
                var right = Factor();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression Factor()
        {
            var expr = Primary();

            while (Match(TokenType.Multiply) || Match(TokenType.Divide))
            {
                var op = Previous();
                var right = Primary();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression Primary()
        {
            if (Match(TokenType.Number)) return new LiteralExpression(Previous().Value);
            if (Match(TokenType.String)) return new LiteralExpression(Previous().Value);
            if (Match(TokenType.Boolean)) return new LiteralExpression(Previous().Value);
            if (Match(TokenType.Null)) return new LiteralExpression(null);

            if (Match(TokenType.Identifier))
            {
                string name = Previous().Value.ToString()!;
                IExpression expr = new VariableExpression(name, _environment);

                if (Match(TokenType.LeftParen))
                {
                    var arguments = new List<IExpression>();
                    if (!Check(TokenType.RightParen))
                    {
                        do
                        {
                            arguments.Add(Expression());
                        } while (Match(TokenType.Comma));
                    }
                    Consume(TokenType.RightParen, "Expected ')' after arguments");
                    return new CallExpression(name, arguments);
                }
                else if (Match(TokenType.LeftBracket))
                {
                    var index = Expression();
                    Consume(TokenType.RightBracket, "Expected ']' after array index");
                    return new Core.Expressions.IndexExpression(expr, index);
                }

                return expr;
            }

            if (Match(TokenType.LeftBracket))
            {
                var elements = new List<IExpression>();
                if (!Check(TokenType.RightBracket))
                {
                    do
                    {
                        elements.Add(Expression());
                    } while (Match(TokenType.Comma));
                }
                Consume(TokenType.RightBracket, "Expected ']' after array elements");
                return new ArrayExpression(elements);
            }

            if (Match(TokenType.LeftParen))
            {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expected ')' after expression");
                return expr;
            }

            throw new Exception($"Unexpected token: {Peek()}");
        }

        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw new Exception($"{message} at token {Peek()}");
        }
    }
}