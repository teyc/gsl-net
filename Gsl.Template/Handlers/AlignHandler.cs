using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Gsl.Infrastructure;
using static System.StringComparison;

namespace Gsl.Handlers
{
    public class AlignHandler : IHandler
    {
        private readonly Dictionary<int, int[]> _alignments = new Dictionary<int, int[]>();
        private readonly ILogger _logger;
        private int _lineNumberOfLastAlignmentRule = -1;

        public AlignHandler(ILogger logger)
        {
            _logger = logger;
        }

        public (bool handled, string js) Handle(int lineNumber, string line)
        {
            if (line is null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }

            if (line.StartsWith(". ", InvariantCulture))
            {
                if (Regex.Match(line.Substring(1), "^[| ]+$").Success)
                {
                    _lineNumberOfLastAlignmentRule = lineNumber;
                    line = "." + line; // put the dot back in
                    _alignments[lineNumber] = line.Split('|')
                        .SkipLast(1)
                        .Select((s, index) => index == 0 ? s.Length - 1 : s.Length + 1)
                        .ToArray();
                    _logger.LogTrace("Alignments: {Alignments}", string.Join(",", _alignments[lineNumber]));
                    return (true, "");
                }
            }

            if (lineNumber == _lineNumberOfLastAlignmentRule + 1)
            {
                var tokens = ParseInterpolatedStringWithAlignment(_lineNumberOfLastAlignmentRule, line);
                var cmd = $"outputAligned({_lineNumberOfLastAlignmentRule}, {string.Join(" + ", tokens.Select(token => token.ToString()))});";
                _lineNumberOfLastAlignmentRule = -1;
                return (true, cmd);
            }

            _lineNumberOfLastAlignmentRule = -1;
            return (false, "");
        }

        public void WriteTo(AddOutput addOutput, object[] args)
        {
            if (addOutput is null)
            {
                throw new System.ArgumentNullException(nameof(addOutput));
            }

            if (args is null)
            {
                throw new System.ArgumentNullException(nameof(args));
            }

            if (args.Length < 2)
            {
                throw new System.ArgumentException("args should have 2 parameters", nameof(args));
            }

            var alignmentId = (int)args[0];
            var lineToBeAligned = (string)args[1];
            addOutput(new OutputBuffer.Aligned(alignmentId, lineToBeAligned));
        }

        internal Token[] ParseInterpolatedStringWithAlignment(int alignmentId, string line)
        {
            using var scope = _logger.BeginScope(nameof(ParseInterpolatedStringWithAlignment));
            var tokens = new List<Token>();
            var startPos = 0;
            foreach (var size in _alignments[alignmentId])
            {
                int endPos = startPos + size;
                var substring = line.Substring(startPos, endPos - startPos);
                tokens.AddRange(TemplateParser.ParseInterpolatedString(substring));
                tokens.Add(StringToken.NULL);
                startPos = endPos;
                _logger.LogTrace("Tokens {tokens}", string.Join(":", tokens.Select(token => token.ToString())));
            }
            tokens.AddRange(TemplateParser.ParseInterpolatedString(line.Substring(startPos)));
            return tokens.ToArray();
        }
    }
}