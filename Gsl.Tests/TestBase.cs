using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using ApprovalTests;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Gsl.Tests
{
    [ApprovalTests.Namers.UseApprovalSubdirectory("data")]
    public class TestBase
    {
        const bool DEBUG = false;
        protected readonly ITestOutputHelper _log;
        protected IFileSystem _fileSystem;
        protected  Gsl.Engine _gslEngine;
        protected ILogger _logger;

        public TestBase(ITestOutputHelper log)
        {
            _log = log;

            _logger = LoggerFactory.Create(configure =>
                configure.SetMinimumLevel(LogLevel.Trace).AddConsole())
                .CreateLogger<AlignmentTest>();
            _fileSystem = new MockFileSystem();
            _gslEngine = new Gsl.Engine(new VM(_fileSystem, _logger));
        }

        protected string Read(IFileInfo fileInfo)
        {
            using var stream = fileInfo.OpenText();
            var contents = stream.ReadToEnd();
            if (DEBUG) 
            {
                _log.WriteLine(contents);
            }
            return contents;
        }
    }
}
