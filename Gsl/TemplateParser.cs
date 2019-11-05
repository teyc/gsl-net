using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gsl
{
    public class TemplateParser
    {
        private int _alignNextLine = -1;
        private int _lineNumber = 0;

        public string TranslateLine(string line)
        {
            _lineNumber++;
            line = line.Replace("\r", "");

            if (line.StartsWith(". "))
            {
                if (Regex.Match(line.Substring(1), "^[| ]+$").Success)
                {
                    _alignNextLine = _lineNumber;
                    return "";
                }
                _alignNextLine = -1;
                return line.Substring(2);
            }

            var tokens = ParseInterpolatedString(line);

            if (_alignNextLine != -1)
            {
                var cmd =  $"outputAligned({_alignNextLine}, {string.Join(" + ", tokens.Select(token => token.ToString()))});";
                _alignNextLine = -1;
                return cmd;
            }
            else
            {
                _alignNextLine = -1;
                return "output(" + string.Join(" + ", tokens.Select(token => token.ToString())) + ");";
            }
        }

        internal Gsl.Token[] ParseInterpolatedString(string line)
        {
            var tokens = new List<Token>();
            int posStart = 0;
            while (posStart < line.Length)
            {
                var posLeft = line.IndexOf("${", posStart);
                if (posLeft != -1)
                {
                    var posRight = line.IndexOf("}", posLeft);
                    if (posStart < posLeft) tokens.Add(new StringToken(line.Substring(posStart, posLeft - posStart)));
                    tokens.Add(new ExpressionToken(line.Substring(posLeft + 2, posRight - (posLeft + 2))));
                    posStart = posRight + 1;
                }
                else
                    break;
            }
            tokens.Add(new StringToken(line.Substring(posStart, line.Length - posStart)));
            return tokens.ToArray();
        }

    }

}