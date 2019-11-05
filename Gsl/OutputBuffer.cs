using System.Linq;
using System.Collections.Generic;
using Gsl.Infrastructure;

namespace Gsl
{
    public class OutputBuffer
    {
        readonly List<object> _lines = new List<object>();

        public OutputBuffer(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; }

        public void WriteLine(string s)
        {
            _lines.Add(s);
        }

        public void WriteAligned(int alignmentId, string s)
        {
            _lines.Add(new Aligned(alignmentId, s));
        }

        public void Close()
        {
            var alignmentGroups = _lines.OfType<Aligned>().GroupBy(l => l.AlignmentId);
            foreach (var alignmentGroup in alignmentGroups)
            {
                Aligned.Expand(alignmentGroup);
            }
        }

        public string GetBuffer()
        {
            return string.Join("\r\n", _lines.Select(line => {
                if (line is string) {
                    return (string) line;
                } else if (line is Aligned alignedLine) {
                    return alignedLine.ExpandedValue;
                } else {
                    throw new System.Exception("Unhandled type " + line.GetType());
                }
            }));
        }

        class Aligned
        {
            public Aligned(int alignmentId, string value)
            {
                AlignmentId = alignmentId;
                Value = value;
            }

            public int AlignmentId { get; }

            public string Value { get; }

            public string ExpandedValue { get; private set; }

            public void ExpandAlignment(int[] widths, string formatString = null)
            {
                formatString = formatString ?? CreateFormatString(widths);
                var elements = Value.Split("\0").Cast<object>().ToArray();
                ExpandedValue = string.Format(formatString, elements);
            }

            private static string CreateFormatString(int[] widths)
            {
                var elements = widths.Select((width, index) => "{" + index + ":" + width + "}");
                var formatString = string.Join("", elements);

                return formatString;
            }

            public static void Expand(IEnumerable<Aligned> items)
            {
                var widths = items.Select(item => item.Value.Split("\0").Select(s => s.Length))
                                  .Transpose()
                                  .Select(widths => widths.Max())
                                  .ToArray();

                var formatString = CreateFormatString(widths.ToArray());
                foreach (var item in items)
                {
                    item.ExpandAlignment(widths, formatString);
                }
            }
        }
    }
}