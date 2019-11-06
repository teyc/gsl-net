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
        private readonly Dictionary<string, OutputBuffer> _files = new Dictionary<string, OutputBuffer>();
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;
        private OutputBuffer _currentOutputFile;

        public VM(IFileSystem fileSystem, ILogger logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void SetOutput(string filename)
        {
            using var scope = logger.BeginScope(nameof(SetOutput));

            var outputBuffer = GetCurrentOutputFile();
            if (outputBuffer.Filename == NONAME)
            {
                outputBuffer.SetOutput(filename);
                _currentOutputFile = _files[filename] = new OutputBuffer(filename, fileSystem);
                _files.Remove(NONAME);
            }
            else
            {
                _files[filename] = new OutputBuffer(filename, fileSystem);
            }

        }

        public void WriteLine(object output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));
            _currentOutputFile.WriteLine(output.ToString());
        }

        public void WriteLineAligned(int alignmentId, string output)
        {
            _currentOutputFile.WriteAligned(alignmentId, output);
        }

        public void WriteProtectedSection(string sectionName, string prefix, string suffix)
        {
            _currentOutputFile.WriteProtectedSection(sectionName, prefix, suffix);
        }
        
        private OutputBuffer GetCurrentOutputFile()
        {
            using var scope = logger.BeginScope(nameof(GetCurrentOutputFile));

            if (_currentOutputFile != null) return _currentOutputFile;

            _currentOutputFile = _files.FirstOrDefault(f => f.Value.Filename == NONAME).Value;
            if (_currentOutputFile != null) return _currentOutputFile;

            if (!fileSystem.File.Exists(NONAME))
            {
                logger.LogTrace($"Creating... {NONAME}");
                using var file = fileSystem.File.CreateText(NONAME);
                file.Write("");
                file.Close();
            }
            _currentOutputFile = _files[NONAME] = new OutputBuffer(NONAME, fileSystem);
            return _currentOutputFile;

        }

        public IFileInfo[] GetOutputFiles()
        {
            using var scope = logger.BeginScope(nameof(GetOutputFiles));

            return _files.Values.Cast<OutputBuffer>().Select(ToFileInfo).ToArray();
        }

        IFileInfo ToFileInfo(OutputBuffer buffer)
        {
            buffer.Close();
            var fileInfo = fileSystem.FileInfo.FromFileName(buffer.Filename);
            using (var outputStream = fileSystem.File.CreateText(buffer.Filename))
                outputStream.Write(buffer.GetBuffer());
            return fileInfo;
        }
    }

}