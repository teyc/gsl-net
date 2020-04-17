using System;
using System.Collections.Generic;
using static System.StringComparison;

namespace Gsl
{
    public abstract class Token
    {
        public readonly char Char;

        public string Value { get; }

        protected Token(string value)
        {
            Value = value;
            Char = (value?.Length > 0) ? value[0] : Char.MaxValue;
        }
    }

    public class StringToken : Token
    {
        public static readonly StringToken ALIGN_LEFT = new StringToken("\0");

        public static readonly StringToken OPTIONAL = new StringToken("\a");

        public StringToken(string value) : base(value?.Replace("\r", "") ?? throw new ArgumentNullException(nameof(value)))
        {
        }

        public char Char => base.Char;

        public override bool Equals(object obj)
        {
            return obj is StringToken b && b.Value == Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            var escaped = Value
                .Replace("'", @"\'")
                .Replace(@"\", @"\\");
            return $"__expandText('{escaped}')";
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
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}