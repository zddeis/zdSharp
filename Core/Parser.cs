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

            if (!Check(TokenType.Else)) {
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

            return new ExpressionStatement(Expression());
        }

        private IExpression Expression()
        {
            if (Match(TokenType.Identifier))
            {
                var name = Previous().Value.ToString()!;

                // assignment
                if (Match(TokenType.Equals))
                    return new AssignmentExpression(name, Expression());

                // function call
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

                return new VariableExpression(name, _environment);
                //return Primary();
            }

            return Equality();
        }

        private IExpression Assignment()
        {
            var expr = Equality();

            if (Match(TokenType.Equals))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is VariableExpression varExpr)
                    return new AssignmentExpression(varExpr._name, value);

                throw new Exception($"Invalid assignment target at line {equals.Line}");
            }

            return expr;
        }

        private IExpression Equality()
        {
            var expr = Term();

            while (Match(TokenType.EqualsEquals))
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
            var expr = Comparison();

            while (Match(TokenType.Multiply) || Match(TokenType.Divide))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expressions.BinaryExpression(expr, op, right);
            }

            return expr;
        }

        private IExpression Comparison()
        {
            var expr = Primary();

            while(Match(TokenType.Or) || Match(TokenType.And))
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

            if (Match(TokenType.Identifier))
            {
                string name = Previous().Value.ToString()!;

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

                return new VariableExpression(name, _environment);
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
