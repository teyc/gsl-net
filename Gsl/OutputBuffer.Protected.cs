using System.Linq;
using System.IO.Abstractions;
using System;

namespace Gsl
{
    public partial class OutputBuffer
    {
        private class Protected : IOutputBufferElement
        {

            public Protected(string sectionName, string prefix, string suffix)
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
            public string ExpandedValue { get; private set;  }
            
            public static void Expand(Protected protectedSection, IFileSystem fileSystem, string outputPath)
            {
                var humanGeneratedCode = string.Join(Environment.NewLine,
                    fileSystem.File.ReadAllLines(outputPath)
                    .SkipWhile(line => line.Trim() != protectedSection.MarkBegin)
                    .Skip(1)
                    .TakeWhile(line => line.Trim() != protectedSection.MarkEnd));

                protectedSection.ExpandedValue = string.Join(Environment.NewLine,
                    protectedSection.MarkBegin,
                    humanGeneratedCode ?? "",
                    protectedSection.MarkEnd);
            }
        }
    }
}