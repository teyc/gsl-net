using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Gsl
{
    public class VM
    {
        private const string NONAME = "__.__.__";
        private readonly Dictionary<int, string> _alignments = new Dictionary<int, string>();
        private readonly Dictionary<string, IFileInfo> _files = new Dictionary<string, IFileInfo>();
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;
        private IFileInfo _currentOutputFile;

        public VM(IFileSystem fileSystem, ILogger logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void SetOutput(string filename)
        {
            using var scope = logger.BeginScope(nameof(SetOutput));

            var fileInfo = GetCurrentOutputFile();
            if (fileInfo.Name == NONAME)
            {
                fileSystem.File.Move(NONAME, filename);
                _currentOutputFile = _files[filename] = fileSystem.FileInfo.FromFileName(filename);
                _files.Remove(NONAME);
            }
            else
            {
                _files[filename] = fileSystem.FileInfo.FromFileName(filename);
            }

        }

        public void WriteLine(object output)
        {
            using var s = _currentOutputFile.AppendText();
            s.WriteLine(output);
        }

        public void WriteLineAligned(int alignmentId, string output)
        {
            using var s = _currentOutputFile.AppendText();
            s.WriteLine(output);
        }

        public void SetAlignment(int alignmentId, string alignmentMarkers)
        {
            _alignments[alignmentId] = alignmentMarkers;
        }

        private IFileInfo GetCurrentOutputFile()
        {
            using var scope = logger.BeginScope(nameof(GetCurrentOutputFile));

            if (_currentOutputFile != null) return _currentOutputFile;

            _currentOutputFile = _files.FirstOrDefault(f => f.Value.Name == NONAME).Value;
            if (_currentOutputFile != null) return _currentOutputFile;

            if (!fileSystem.File.Exists(NONAME))
            {
                logger.LogTrace($"Creating... {NONAME}");
                using var file = fileSystem.File.CreateText(NONAME);
                file.Write("");
                file.Close();
            }
            _currentOutputFile = _files[NONAME] = fileSystem.FileInfo.FromFileName(NONAME);
            return _currentOutputFile;

        }

        public IFileInfo[] GetOutputFiles()
        {
            using var scope = logger.BeginScope(nameof(GetOutputFiles));

            return _files.Values.Cast<IFileInfo>().ToArray();
        }
    }

}