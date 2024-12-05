using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace zdSharp
{
    public class Token
    {
        public string Type { get; }
        public dynamic Value { get; }
        public int Line { get; }

        public Token(string type, dynamic value)
        {
            Type = type;
            Line = Global.CurrentLine;
            Value = value is char ? value.ToString() : value;
        }

        public override string ToString() => $"{Type} : {Value} : {Line}";
    }

    public class Lexer
    {
        private static readonly string[] _Keywords = { "set", "print", "if", "else", "return", "continue", "break", "for", "while", "do", "function" };
        private static readonly string[] _Functions = { "print", "WaitKey" };
        private static readonly string[] _Equal = { "=" };
        private static readonly string[] _Bools = { "true", "false" };
        private static readonly string[] _Operators = { "+", "-", "*", "/" };

        private static readonly string[] _Comparators = { "==", ">=", "<=", "!=", ">", "<" };
        private static readonly string[] _Comparators_Chars = { "=", ">", "<", "!", ">", "<" };
        private static readonly string[] _Parentheses = { "(", ")" };
        private static readonly string[] _QuotationMarks = { "'", "\"" };


        private readonly string _input;
        private int _position;
        private int _line;

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (!IsAtEnd())
            {
                var current = NextPos();

                if (char.IsWhiteSpace(current)){
                    continue;
                }

                if (char.IsLetter(current))
                {
                    var identifier = ReadWhile(IsIdentifier);

                    if (_Bools.Contains(identifier)){
                        tokens.Add(new Token("Bool", identifier));
                        continue;
                    }

                    if (_Keywords.Contains(identifier)){
                        tokens.Add(new Token("Keyword", identifier));
                        continue;
                    }

                    if (_Functions.Contains(identifier))
                    {
                        tokens.Add(new Token("Function", identifier));
                        continue;
                    }

                    tokens.Add(new Token("Identifier", identifier));
                    continue;
                }
                
                if (char.IsDigit(current))
                {
                    var number = ReadWhile(IsNumber);

                    tokens.Add(new Token("Number", number));
                    continue;
                }

                if (_Comparators_Chars.Contains(current.ToString()))
                {
                    var comparator = current.ToString();

                    if (!IsAtEnd())
                    {
                        comparator += NextPos();

                        if (_Comparators.Contains(comparator))
                        {
                            tokens.Add(new Token("Comparator", comparator));
                            continue;
                        }

                        PrevPos();
                    }
                }

                if (_Equal.Contains(current.ToString()))
                {
                    tokens.Add(new Token("Equal", current));
                    continue;
                }

                if (_Operators.Contains(current.ToString()))
                {
                    tokens.Add(new Token("Operator", current));
                    continue;
                }

                if (_Parentheses.Contains(current.ToString()))
                {
                    tokens.Add(new Token("Parentheses", current));
                    continue;
                }

                if (_QuotationMarks.Contains(current.ToString()))
                {
                    string s = "";
                    current = NextPos();

                    while (!IsAtEnd() && !_QuotationMarks.Contains(current.ToString()))
                    {
                        s += current;
                        current = NextPos();
                    }

                    tokens.Add(new Token("String", s));
                    continue;
                }

                throw new Exception($"Unexpected character: {current}");
            }

            return tokens;
        }

        private bool IsAtEnd() => _position >= _input.Length;
        private bool IsAtStart() => _position == 0;

        private char NextPos() => _input[_position++];
        private char PrevPos() => _input[_position--];

        private char Peek() => IsAtEnd() ? '\0' : _input[_position];

        private bool IsNumber(char c) => char.IsDigit(c) || c == '.';
        private bool IsIdentifier(char c) => char.IsLetterOrDigit(c) || c == '.';
        
        private string ReadWhile(Func<char, bool> predicate)
        {
            var start = _position - 1;

            while (!IsAtEnd() && predicate(Peek()))
                NextPos();

            return _input.Substring(start, _position - start);
        }
    }
}
