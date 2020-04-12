using System.Linq;
using System.IO.Abstractions;
using System;
using System.Collections.Generic;

namespace Gsl
{
    public partial class OutputBuffer
    {
        internal class ProtectedSection : IOutputBufferElement
        {
            public ProtectedSection(string sectionName, string prefix, string suffix)
            {
                SectionName = sectionName;
                Prefix = prefix;
                Suffix = suffix;
                MarkBegin = $"{prefix} CUSTOM-CODE-BEGIN {sectionName} {suffix}".Trim();
                MarkEnd = $"{prefix} CUSTOM-CODE-END {sectionName} {suffix}".Trim();
            }

            public string SectionName { get; }
            public string Prefix { get; }
            public string Suffix { get; }
            public string MarkBegin { get; }
            public string MarkEnd { get; }
            public string ExpandedValue { get; private set; }

            public static void Expand(ProtectedSection protectedSection, IFileSystem fileSystem, string outputPath)
            {
                var humanGeneratedCode = fileSystem.File.Exists(outputPath)
                    ? fileSystem.File.ReadAllLines(outputPath)
                      .SkipWhile(line => line.Trim() != protectedSection.MarkBegin)
                      .Skip(1)
                      .TakeWhile(line => line.Trim() != protectedSection.MarkEnd)
                      .ToList()
                    : new List<string>();

                var maxLeftIndent = humanGeneratedCode.Count > 0
                    ? humanGeneratedCode.Min(line => line.Length - line.TrimStart().Length)
                    : 0;

                var indent = string.Format(System.Globalization.CultureInfo.InvariantCulture, $"{{0,{maxLeftIndent}}}", "");
                protectedSection.ExpandedValue = string.Join(Environment.NewLine,
                    indent + protectedSection.MarkBegin,
                    string.Join(Environment.NewLine, humanGeneratedCode),
                    indent + protectedSection.MarkEnd);
            }
        }
    }
}