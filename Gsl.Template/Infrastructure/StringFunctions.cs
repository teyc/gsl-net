using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Gsl
{
    public static class StringFunctions
    {
        public static string KebabCase(string properCase)
        {
            return string.Concat(properCase
                .Select((ch, index) => index == 0
                    ? char.ToString(char.ToLowerInvariant(ch))
                    : Char.IsUpper(ch)
                        ? "-" + char.ToLowerInvariant(ch)
                        : char.ToString(ch)));
        }

        public static string CamelCase(string properCase)
        {
            if (properCase is null)
            {
                throw new ArgumentNullException(nameof(properCase));
            }

            return char.ToUpperInvariant(properCase[0]) + properCase.Substring(1);
        }
    }
}