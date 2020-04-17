using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions;
using System;
using Gsl.Handlers;

namespace Gsl
{
    public partial class OutputBuffer
    {
        private bool _isClosed;
        private readonly List<object> _lines = new List<object>();
        private readonly IFileSystem _fileSystem;
        private readonly AlignHandler _alignHandler;
        private readonly ProtectedHandler _protectedHandler = new ProtectedHandler();

        public OutputBuffer(string filename, IFileSystem fileSystem, AlignHandler alignHandler)
        {
            Filename = filename;
            _fileSystem = fileSystem;
            _alignHandler = alignHandler;
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
            var alignmentGroups = _lines.OfType<Aligned>().GroupBy(l => l.AlignmentId).ToArray();
            foreach (var alignmentGroup in alignmentGroups)
            {
                Aligned.Expand(alignmentGroup);
            }

            var protectedSections = _lines.OfType<ProtectedSection>();
            foreach (var protectedSection in protectedSections)
            {
                ProtectedSection.Expand(protectedSection, _fileSystem, Filename);
            }

            _isClosed = true;
        }

        private void WriteWithHandler(IHandler handler, params object[] arguments)
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
            if (!_isClosed) throw new InvalidOperationException("Close() must be called before GetBuffer");

            return string.Join("\r\n", _lines.Select(line =>
            {
                if (line is string @string)
                {
                    return @string;
                }
                else if (line is IOutputBufferElement element)
                {
                    return element.ExpandedValue;
                }
                else
                {
                    throw new System.Exception("Unhandled type " + line.GetType());
                }
            }));
        }
    }
}