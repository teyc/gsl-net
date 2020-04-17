using System;
using System.Collections.Generic;
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
        public static readonly StringToken NULL = new StringToken("\0");

        public StringToken(string value) : base(value?.Replace("\r", "") ?? throw new ArgumentNullException(nameof(value)))
        {
        }

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

    public class OptionalToken : StringToken
    {
        public OptionalToken(string value) : base(value)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is OptionalToken token &&
                   base.Equals(obj) &&
                   Value == token.Value;
        }

        public override int GetHashCode()
        {
            int hashCode = -159790080;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public override string ToString()
        {
            var escaped = Value
                .Replace("'", @"\'")
                .Replace(@"\", @"\\");
            return $"__optionalText('{escaped}')";
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