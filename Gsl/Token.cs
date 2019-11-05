using System;

namespace Gsl
{
    public abstract class Token
    {
        public string Value { get; }

        public Token(string value)
        {
            Value = value;
        }

    }

    public class StringToken : Token
    {
        public StringToken(string value) : base(value.Replace("\r", ""))
        {
        }

        public override string ToString()
        {
            var escaped = Value
                .Replace("'", @"\'")
                .Replace(@"\", @"\\");
            return $"'{escaped}'";
        }
    }

    public class ExpressionToken : Token
    {
        public ExpressionToken(string value) : base(value)
        {
        }

        public override string ToString()
        {
            return Value;
        }
    }

}