using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Gsl.Tests")]

namespace Gsl
{
    public class TemplateParser
    {
        private readonly Dictionary<int, int[]> _alignments = new Dictionary<int, int[]>();
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
                    line = "." + line; // put the dot back in
                    _alignments[_lineNumber] = line.Split('|')
                        .SkipLast(1)
                        .Select((s, index) => index == 0? s.Length - 1 : s.Length + 1)
                        .ToArray();
                    return "";
                }
                _alignNextLine = -1;
                return line.Substring(2);
            }

            if (_alignNextLine != -1)
            {
                var tokens = ParseInterpolatedStringWithAlignment(_alignNextLine, line);
                var cmd =  $"outputAligned({_alignNextLine}, {string.Join(" + ", tokens.Select(token => token.ToString()))});";
                _alignNextLine = -1;
                return cmd;
            }
            else
            {
                _alignNextLine = -1;
                var tokens = ParseInterpolatedString(line);
                return "output(" + string.Join(" + ", tokens.Select(token => token.ToString())) + ");";
            }
        }

        internal Token[] ParseInterpolatedStringWithAlignment(int alignmentId, string line)
        {
            var tokens = new List<Token>();
            var startPos = 0;
            foreach (var size in _alignments[alignmentId]) {
                int endPos = startPos + size;
                var substring = line[startPos..endPos];
                tokens.AddRange(ParseInterpolatedString(substring));
                tokens.Add(new StringToken("\0"));
                startPos = endPos;
            }
            tokens.AddRange(ParseInterpolatedString(line[startPos..]));
            return tokens.ToArray(); 
        }

        internal Token[] ParseInterpolatedString(string line)
        {
            var tokens = new List<Token>();
            int posStart = 0;
            while (posStart < line.Length)
            {
                var posLeft = line.IndexOf("${", posStart);
                if (posLeft != -1)
                {
                    var posRight = line.IndexOf("}", posLeft);
                    if (posStart < posLeft) tokens.Add(new StringToken(line[posStart..posLeft]));
                    tokens.Add(new ExpressionToken(line[(posLeft + 2)..posRight]));
                    posStart = posRight + 1;
                }
                else
                    break;
            }
            tokens.Add(new StringToken(line[posStart..]));
            return tokens.ToArray();
        }

    }

}