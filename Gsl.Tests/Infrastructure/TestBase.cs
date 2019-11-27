using System.Linq;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using static Gsl.Tests.Path;

namespace Gsl.Tests
{
    [ApprovalTests.Namers.UseApprovalSubdirectory("data")]
    [UseReporter(typeof(DiffReporter))]
    public class TestBase
    {
        const bool DEBUG = false;
        protected readonly ITestOutputHelper _log;
        protected IFileSystem _fileSystem;
        protected Engine _gslEngine;
        protected ILogger _logger;

        public TestBase(ITestOutputHelper log)
        {
            _log = log;

            _logger = LoggerFactory.Create(configure =>
                configure.SetMinimumLevel(LogLevel.Trace).AddConsole())
                .CreateLogger<AlignmentTest>();
            _fileSystem = new MockFileSystem();
            _gslEngine = new Engine(new VM(_fileSystem, _logger));
        }

        protected void TemplateWithSingleFileOutput(string template, string data)
        {
            var outputFiles = _gslEngine.Execute(
                new FileInfoWrapper(_fileSystem, new FileInfo(DataFile(template))),
                new FileInfoWrapper(_fileSystem, new FileInfo(DataFile(data))));

            outputFiles.ToList().ForEach(f => _log.WriteLine(f.Name));
            var outputFile = outputFiles[0];
            var outputContents = outputFile.ReadToEnd();

            Approvals.Verify(outputContents);
        }

        protected string Read(IFileInfo fileInfo)
        {
            using var stream = fileInfo.OpenText();
            var contents = stream.ReadToEnd();
            if (DEBUG)
            {
                #pragma warning disable CS0162
                _log.WriteLine(contents);
                #pragma warning restore CS0162
            }
            return contents;
        }
    }
}
