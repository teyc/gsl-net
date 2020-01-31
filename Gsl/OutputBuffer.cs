using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions;
using System;
using Gsl.Handlers;

namespace Gsl
{
    public partial class OutputBuffer
    {
        readonly List<object> _lines = new List<object>();
        private readonly IFileSystem fileSystem;
        private readonly AlignHandler _alignHandler = new AlignHandler();
        private readonly ProtectedHandler _protectedHandler = new ProtectedHandler();

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

        void WriteWithHandler(IHandler handler, params object[] arguments)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            handler.WriteTo(this._lines.Add, arguments);
        }

        public void WriteAligned(int alignmentId, string s)
        {
            WriteWithHandler(_alignHandler, alignmentId, s);
        }

        public void WriteProtectedSection(string sectionName, string prefix, string suffix)
        {
            WriteWithHandler(_protectedHandler, sectionName, prefix, suffix);
        }

        public string GetBuffer()
        {
            return string.Join("\r\n", _lines.Select(line => {
                if (line is string @string) {
                    return @string;
                } else if (line is IOutputBufferElement element) {
                    return element.ExpandedValue;
                } else {
                    throw new System.Exception("Unhandled type " + line.GetType());
                }
            }));
        }
    }
}