using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core
{
    public class Token
    {
        public TokenType Type { get; }
        public object Value { get; }
        public int Line { get; }

        public Token(TokenType type, object value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }

        public override string ToString()
        {
            return Type.ToString() + ", " + Value.ToString() + ", " + Line;
        }
    }

    public enum TokenType
    {
        // Keywords
        Function,
        While,
        End,
        If,
        Then,
        Else,
        Return,

        // Literals
        Number,
        String,
        Boolean,
        Identifier,

        // Operators
        Plus,
        Minus,
        Multiply,
        Divide,
        Equals,

        EqualsEquals,
        Or,
        And,

        // Delimiters
        LeftParen,
        RightParen,
        LeftBracket,
        RightBracket,
        Comma,

        // End of File
        EOF
    }
}
