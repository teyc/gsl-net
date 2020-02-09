using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Gsl
{
    public class VM
    {
        private const string NONAME = "__.__.__";
        private readonly Dictionary<string, OutputBuffer> _files = new Dictionary<string, OutputBuffer>();
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;
        private OutputBuffer _currentOutputFile;
        private (string Search, string FileExtension) doNotOverwrite;

        public VM(IFileSystem fileSystem, ILogger logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void DoNotOverwriteIf(string searchString, string fileExtension)
        {
            this.doNotOverwrite = (Search: searchString, FileExtension: fileExtension);
        }

        public void SetOutput(string filename)
        {
            using var scope = logger.BeginScope(nameof(SetOutput));

            var outputBuffer = GetCurrentOutputFile();
            if (outputBuffer.Filename == NONAME)
            {
                outputBuffer.SetOutput(filename);
                _currentOutputFile = _files[filename] = new OutputBuffer(filename, fileSystem, new Handlers.AlignHandler(logger));
                _files.Remove(NONAME);
            }
            else
            {
                _files[filename] = new OutputBuffer(filename, fileSystem, new Handlers.AlignHandler(logger));
            }
        }

        public void WriteLine(object output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (_currentOutputFile == null) GetCurrentOutputFile(); // throw new OutputFileNotDefinedException();
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
                logger.LogTrace("Creating... {0}", NONAME);
                using var file = fileSystem.File.CreateText(NONAME);
                file.Write("");
                file.Close();
            }
            _currentOutputFile = _files[NONAME] = new OutputBuffer(NONAME, fileSystem, new Handlers.AlignHandler(logger));
            return _currentOutputFile;
        }

        public IFileInfo[] GetOutputFiles()
        {
            using var scope = logger.BeginScope(nameof(GetOutputFiles));

            _files.Values.Cast<OutputBuffer>()
                .Where(f => Path.GetFileName(f.Filename) == NONAME)
                .Select(f => { f.Close(); return f.GetBuffer(); })
                .ToList()
                .ForEach(Console.WriteLine);

            return _files.Values.Cast<OutputBuffer>()
                .Where(f => Path.GetFileName(f.Filename) != NONAME)
                .Select(ToFileInfo).ToArray();
        }

        private IFileInfo ToFileInfo(OutputBuffer buffer)
        {
            using var scope = logger.BeginScope(nameof(ToFileInfo));

            buffer.Close();
            var fileInfo = fileSystem.FileInfo.FromFileName(buffer.Filename);
            if (doNotOverwrite != default
                && fileSystem.File.Exists(buffer.Filename)
                && fileSystem.File.ReadAllText(buffer.Filename).Contains(doNotOverwrite.Search, StringComparison.InvariantCulture))
            {
                var alternateFilename = buffer.Filename + doNotOverwrite.FileExtension;
                logger.LogWarning("Not overwriting {Filename} because {protection} found. Wrote to {AlternateFilename}",
                    fileInfo.Name,
                    doNotOverwrite.Search,
                    alternateFilename);
                using (var outputStream = fileSystem.File.CreateText(alternateFilename))
                    outputStream.Write(buffer.GetBuffer());
                return fileInfo;
            }
            else
            {
                logger.LogTrace("Outputting {Filename}", fileInfo.Name);
                using (var outputStream = fileSystem.File.CreateText(buffer.Filename))
                    outputStream.Write(buffer.GetBuffer());
                return fileInfo;
            }
        }
    }
}