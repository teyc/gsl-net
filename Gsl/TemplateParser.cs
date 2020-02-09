using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using static System.StringComparison;
using Gsl.Handlers;

[assembly: InternalsVisibleTo("Gsl.Tests")]

namespace Gsl
{
    public class TemplateParser
    {
        private readonly AlignHandler _alignHandler;
        private readonly ReplaceTextPreprocessor _replaceTextPreprocessor;
        private readonly JsHandler _jsHandler = new JsHandler();

        private int _lineNumber = 0;

        public TemplateParser(AlignHandler alignHandler, ReplaceTextPreprocessor replaceTextPreprocessor)
        {
            _alignHandler = alignHandler;
            _replaceTextPreprocessor = replaceTextPreprocessor;
        }

        public string TranslateLine(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            _lineNumber++;
            line = line.Replace("\r", "", InvariantCulture);

            var (ok, result) = _replaceTextPreprocessor.Handle(_lineNumber, line);
            if (ok) return result;

            (ok, result) = _alignHandler.Handle(_lineNumber, line);
            if (ok) return result;

            (ok, result) = _jsHandler.Handle(_lineNumber, line);
            if (ok) return result;

            var tokens = ParseInterpolatedString(line, _replaceTextPreprocessor);
            return "output(" + string.Join(" + ", tokens.Select(token => token.ToString())) + ");";
        }

        internal static Token[] ParseInterpolatedString(string line, ReplaceTextPreprocessor replaceTextPreprocessor)
        {
            var tokens = new List<Token>();
            int posStart = 0;
            while (posStart < line.Length)
            {
                var posLeft = line.IndexOf("${", posStart, InvariantCulture);
                if (posLeft != -1)
                {
                    var posRight = line.IndexOf("}", posLeft, InvariantCulture);
                    if (posStart < posLeft)
                    {
                        tokens.Add(new StringToken(
                            replaceTextPreprocessor?.Expand(line[posStart..posLeft]) ?? line[posStart..posLeft]));
                    }

                    tokens.Add(new ExpressionToken(line[(posLeft + 2)..posRight]));
                    posStart = posRight + 1;
                }
                else
                {
                    break;
                }
            }
            tokens.Add(new StringToken(replaceTextPreprocessor?.Expand(line[posStart..]) ?? line[posStart..]));
            return tokens.ToArray();
        }

        internal static IEnumerable<Token> ParseInterpolatedString(string line)
        {
            return ParseInterpolatedString(line, null);
        }
    }
}