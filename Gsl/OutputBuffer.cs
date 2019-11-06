using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions;
using System;

namespace Gsl
{
    public partial class OutputBuffer
    {
        readonly List<object> _lines = new List<object>();
        private readonly IFileSystem fileSystem;

        public OutputBuffer(string filename, IFileSystem fileSystem)
        {
            Filename = filename;
            this.fileSystem = fileSystem;
        }

        public string Filename { get; private set; }

        public void SetOutput(string filename)
        {
            Filename = filename;
        }

        public void WriteLine(string s)
        {
            _lines.Add(s);
        }

        public void Close()
        {
            var alignmentGroups = _lines.OfType<Aligned>().GroupBy(l => l.AlignmentId);
            foreach (var alignmentGroup in alignmentGroups)
            {
                Aligned.Expand(alignmentGroup);
            }

            var protectedSections = _lines.OfType<Protected>();
            foreach (var protectedSection in protectedSections)
            {
                Protected.Expand(protectedSection, fileSystem, Filename);
            }
        }

        public void WriteAligned(int alignmentId, string s)
        {
            _lines.Add(new Aligned(alignmentId, s));
        }

        public void WriteProtectedSection(string sectionName, string prefix, string suffix)
        {
            _lines.Add(new Protected(sectionName, prefix, suffix));
        }

        public string GetBuffer()
        {
            return string.Join("\r\n", _lines.Select(line => {
                if (line is string) {
                    return (string) line;
                } else if (line is IOutputBufferElement element) {
                    return element.ExpandedValue;
                } else {
                    throw new System.Exception("Unhandled type " + line.GetType());
                }
            }));
        }
    }
}