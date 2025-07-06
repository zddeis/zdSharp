using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Exceptions;
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
            var expr = LogicalOr();

            if (Match(TokenType.Equals))
            {
                var value = Expression();
                int line = Previous().Line;

                if (expr is VariableExpression variable)
                {
                    return new AssignmentExpression(variable._name, value, _environment, line);
                }
                else if (expr is Expressions.IndexExpression indexExpr)
                {
                    return new IndexAssignmentExpression(
                        ((Expressions.IndexExpression)expr)._array,
                        ((Expressions.IndexExpression)expr)._index,
                        value,
                        line);
                }

                throw new ParseException("Invalid assignment target", line);
            }

            return expr;
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
            Token current = Peek();
            int line = current.Line;

            if (Match(TokenType.Number)) return new LiteralExpression(Previous().Value, line);
            if (Match(TokenType.String)) return new LiteralExpression(Previous().Value, line);
            if (Match(TokenType.Boolean)) return new LiteralExpression(Previous().Value, line);
            if (Match(TokenType.Null)) return new LiteralExpression(null, line);

            if (Match(TokenType.Identifier))
            {
                string name = Previous().Value.ToString()!;
                IExpression expr = new VariableExpression(name, _environment, line);

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
                    return new CallExpression(name, arguments, line);
                }
                else if (Match(TokenType.LeftBracket))
                {
                    var index = Expression();
                    Consume(TokenType.RightBracket, "Expected ']' after array index");

                    // Check if this is an assignment to an array element
                    if (Match(TokenType.Equals))
                    {
                        var value = Expression();
                        return new IndexAssignmentExpression(expr, index, value, line);
                    }

                    return new Core.Expressions.IndexExpression(expr, index, line);
                }
                else if (Match(TokenType.Period))
                {
                    var property = Consume(TokenType.Identifier, "Expected property name after '.'").Value.ToString()!;

                    // Check if this is a method call
                    if (Check(TokenType.LeftParen))
                    {
                        Advance(); // Consume the left parenthesis
                        var arguments = new List<IExpression>();
                        if (!Check(TokenType.RightParen))
                        {
                            do
                            {
                                arguments.Add(Expression());
                            } while (Match(TokenType.Comma));
                        }
                        Consume(TokenType.RightParen, "Expected ')' after arguments");

                        // Create a method call expression
                        return new Expressions.MethodCallExpression(expr, property, arguments, line);
                    }

                    // Check if this is a property assignment
                    if (Match(TokenType.Equals))
                    {
                        var value = Expression();
                        return new PropertyExpression(expr, property, value, line);
                    }

                    return new PropertyAccessExpression(expr, property, line);
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
                return new ArrayExpression(elements, line);
            }

            if (Match(TokenType.LeftParen))
            {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expected ')' after expression");
                return expr;
            }

            throw new ParseException($"Unexpected token: {Peek()}", line);
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
            throw new ParseException(message, Peek().Line);
        }
    }
}