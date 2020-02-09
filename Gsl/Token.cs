using System;
using static System.StringComparison;

namespace Gsl
{
    public abstract class Token
    {
        public string Value { get; }

        protected Token(string value)
        {
            Value = value;
        }
    }

    public class StringToken : Token
    {
        public StringToken(string value) : base(value?.Replace("\r", "", InvariantCulture) ?? throw new ArgumentNullException(nameof(value)))
        {
        }

        public override bool Equals(object obj)
        {
            return obj is StringToken b && b.Value == Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override string ToString()
        {
            var escaped = Value
                .Replace("'", @"\'", InvariantCulture)
                .Replace(@"\", @"\\", InvariantCulture);
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
            return HashCode.Combine(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}