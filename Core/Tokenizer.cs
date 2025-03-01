using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zds.Core;

namespace zds.Core
{
    public class Tokenizer
    {
        private readonly string _input;
        private int _position;
        private int _line = 1;
        private readonly List<Token> _tokens = new();

        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            ["function"] = TokenType.Function,
            ["end"] = TokenType.End,
            ["then"] = TokenType.Then,
            ["while"] = TokenType.While,
            ["if"] = TokenType.If,
            ["else"] = TokenType.Else,
            ["return"] = TokenType.Return,
            ["true"] = TokenType.Boolean,
            ["false"] = TokenType.Boolean,
            ["null"] = TokenType.Null,
            ["for"] = TokenType.For,
            ["to"] = TokenType.To,
            ["step"] = TokenType.Step
        };

        public Tokenizer(string input)
        {
            _input = input.Trim();
        }

        public List<Token> Tokenize()
        {
            while (_position < _input.Length)
            {
                char c = Peek();

                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n') _line++;
                    _position++;
                    continue;
                }

                if (char.IsDigit(c) || (c == '-' && char.IsDigit(Peek(1))))
                {
                    TokenizeNumber();
                    continue;
                }

                if (char.IsLetter(c) || c == '_')
                {
                    TokenizeIdentifier();
                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    TokenizeString();
                    continue;
                }

                if (c == '/' && Peek(1) == '/')
                {
                    while (_position < _input.Length && Peek() != '\n')
                        _position++;
                    continue;
                }

                TokenizeSymbol();
            }

            _tokens.Add(new Token(TokenType.EOF, "", _line));

            return _tokens;
        }

        private char Peek(int offset = 0)
        {
            var pos = _position + offset;
            return pos < _input.Length ? _input[pos] : '\0';
        }

        private void TokenizeNumber()
        {
            var number = new StringBuilder();
            while (_position < _input.Length && (char.IsDigit(Peek()) || Peek() == '-' || Peek() == '.'))
            {
                number.Append(_input[_position++]);
            }
            _tokens.Add(new Token(TokenType.Number, double.Parse(number.ToString(), CultureInfo.InvariantCulture), _line));
        }

        private void TokenizeIdentifier()
        {
            var identifier = new StringBuilder();
            while (_position < _input.Length && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
            {
                identifier.Append(_input[_position++]);
            }

            string word = identifier.ToString();
            if (Keywords.TryGetValue(word, out TokenType type))
            {
                object value = type == TokenType.Boolean ? bool.Parse(word) :
                               type == TokenType.Null ? null : word;
                _tokens.Add(new Token(type, value, _line));
            }
            else
            {
                _tokens.Add(new Token(TokenType.Identifier, word, _line));
            }
        }

        private void TokenizeString()
        {
            char quote = _input[_position++];
            string str = "";
            while (_position < _input.Length && Peek() != quote)
            {
                if (Peek() == '\\' && _position + 1 < _input.Length)
                {
                    _position++;
                    switch (Peek())
                    {
                        case 'n': str += "\n"; break;
                        case 't': str += "\t"; break;
                        case 'r': str += "\r"; break;
                        default: str += Peek(); _position--; break;
                    }
                    _position++;
                    continue;
                }

                str += Peek();
                _position++;
            }
            if (_position < _input.Length) _position++; // Skip closing quote
            _tokens.Add(new Token(TokenType.String, str.ToString(), _line));
        }

        private void TokenizeSymbol()
        {
            char c = _input[_position++];
            switch (c)
            {
                case '+': _tokens.Add(new Token(TokenType.Plus, "+", _line)); break;
                case '-': _tokens.Add(new Token(TokenType.Minus, "-", _line)); break;
                case '*': _tokens.Add(new Token(TokenType.Multiply, "*", _line)); break;
                case '/': _tokens.Add(new Token(TokenType.Divide, "/", _line)); break;
                case '|': _tokens.Add(new Token(TokenType.Or, "|", _line)); break;
                case '&': _tokens.Add(new Token(TokenType.And, "&", _line)); break;
                case '.': _tokens.Add(new Token(TokenType.Period, ".", _line)); break;
                case '=':
                    if (Peek() == '=')
                    {
                        _position++;
                        _tokens.Add(new Token(TokenType.EqualsEquals, "==", _line));
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Equals, "=", _line));
                    }
                    break;
                case '!':
                    if (Peek() == '=')
                    {
                        _position++;
                        _tokens.Add(new Token(TokenType.NotEquals, "!=", _line));
                    }
                    else
                    {
                        throw new Exception($"Expected '=' after '!' at line {_line}");
                    }
                    break;
                case '>':
                    if (Peek() == '=')
                    {
                        _position++;
                        _tokens.Add(new Token(TokenType.GreaterEquals, ">=", _line));
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Greater, ">", _line));
                    }
                    break;
                case '<':
                    if (Peek() == '=')
                    {
                        _position++;
                        _tokens.Add(new Token(TokenType.LessEquals, "<=", _line));
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Less, "<", _line));
                    }
                    break;
                case '(': _tokens.Add(new Token(TokenType.LeftParen, "(", _line)); break;
                case ')': _tokens.Add(new Token(TokenType.RightParen, ")", _line)); break;
                case '[': _tokens.Add(new Token(TokenType.LeftBracket, "[", _line)); break;
                case ']': _tokens.Add(new Token(TokenType.RightBracket, "]", _line)); break;
                case ',': _tokens.Add(new Token(TokenType.Comma, ",", _line)); break;
                default:
                    throw new Exception($"Unexpected character: {c} at line {_line}");
            }
        }
    }
}