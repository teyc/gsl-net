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

        public override bool Equals(object obj)
        {
            return obj is StringToken b && b.Value == Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        public override bool Equals(object obj)
        {
            return obj is ExpressionToken b && b.Value == Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }

}