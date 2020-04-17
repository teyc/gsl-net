using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gsl.Infrastructure;
using Microsoft.Extensions.Logging;
using static System.StringComparison;

namespace Gsl.Handlers
{

    public class AlignHandler : IHandler
    {
        private readonly Dictionary<int, Block[]> _alignments = new Dictionary<int, Block[]>();
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
                if (Regex.Match(line.Substring(1), "^[?| ]+$").Success)
                {
                    _lineNumberOfLastAlignmentRule = lineNumber;
                    line = "." + line; // put the dot back in
                    var pos = 1;
                    Block[] blocks = line.Split('?', '|')
                                            .Select((s, index) => index == 0 ? s.Length - 1 : s.Length + 1)
                                            .Select((width, index) =>
                                            {
                                                var splitChar = line.Substring(pos, width)[0];
                                                _logger.LogTrace("Blocks `{substring}`", line.Substring(pos, width));
                                                pos += width;
                                                return splitChar == '?'
                                                    ? Block.CreateOptionalBlock(width)
                                                    : Block.CreateAlignmentBlock(width);
                                            })
                                            .ToArray();

                    // sometimes the alignment line length is shorter than the template line length
                    if (blocks.Length > 0)
                    {
                        blocks[blocks.Length - 1] = blocks[blocks.Length - 1].IsOptional
                                                    ? Block.CreateOptionalBlock(5000)
                                                    : Block.CreateAlignmentBlock(5000);
                    }
                    _alignments[lineNumber] = blocks;
                    // _logger.LogTrace("Alignments: {Alignments}", string.Join(",", _alignments[lineNumber]));
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
            foreach (var block in _alignments[alignmentId])
            {
                int endPos = Math.Min(startPos + block.Width, line.Length);
                var substring = line.Substring(startPos, endPos - startPos);
                _logger.LogInformation("{startPos}, {endPos} {substring}", startPos, endPos, substring);
                if (block.IsOptional)
                {
                    tokens.AddRange(TemplateParser.ParseInterpolatedString(substring).Select(ToOptional));
                }
                else
                {
                    tokens.AddRange(TemplateParser.ParseInterpolatedString(substring));
                }
                tokens.Add(StringToken.NULL);
                startPos = endPos;
                _logger.LogTrace("Tokens {tokens}", string.Join(":", tokens.Select(token => token.ToString())));
            }

            if (startPos != line.Length)
            {
                tokens.AddRange(TemplateParser.ParseInterpolatedString(line.Substring(startPos)));
            }

            return tokens.LastOrDefault() == StringToken.NULL
                ? tokens.Take(tokens.Count - 1).ToArray()
                : tokens.ToArray();
        }

        private Token ToOptional(Token token)
        {
            return (token is StringToken stringToken)
                    ? new OptionalToken(stringToken.Value)
                    : token;
        }
    }
}