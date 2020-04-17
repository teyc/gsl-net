using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Gsl.Infrastructure;

namespace Gsl
{
    public partial class OutputBuffer
    {
        internal class Aligned: IOutputBufferElement
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
                formatString ??= CreateFormatString(widths);
                var elements = Value.Split('\0').Cast<object>().ToArray();
                ExpandedValue = string.Format(CultureInfo.InvariantCulture, formatString, elements).TrimEnd();
            }

            private static string CreateFormatString(int[] widths)
            {
                var elements = widths.Select((width, index) => "{" + index + ",-" + width + "}");
                return string.Concat(elements);
            }

            public static void Expand(IEnumerable<Aligned> items)
            {
                var widths = items.Select(item => item.Value.Split('\0').Select(s => s.Length))
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