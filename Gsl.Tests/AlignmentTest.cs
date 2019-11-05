using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using ApprovalTests;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using static Gsl.Tests.Path;

namespace Gsl.Tests
{
    public class AlignmentTest
    {
        private readonly ITestOutputHelper _log;

        public AlignmentTest(ITestOutputHelper log)
        {
            _log = log;
        }

        [Fact]
        public void AlignProperties()
        {
            const string template = "data/align.gsl";
            const string data = "data/align.json";
            const string expected = "data/align.cs.approved";

            var logger = LoggerFactory.Create(configure => 
                configure.SetMinimumLevel(LogLevel.Trace).AddConsole())
                .CreateLogger<AlignmentTest>();
            var fileSystem = new MockFileSystem();
            var engine = new Gsl.Engine(new VM(fileSystem, logger));
            var outputFiles = engine.Execute(
                new FileInfoWrapper(fileSystem, new FileInfo(DataFile(template))), 
                new FileInfoWrapper(fileSystem, new FileInfo(DataFile(data))));

            outputFiles.ToList().ForEach(f =>  _log.WriteLine(f.Name));
            var outputFile = outputFiles[0];

            using var stream = outputFile.OpenText();            
            var outputContents = stream.ReadToEnd();

            _log.WriteLine(outputContents);
            Assert.Equal(File.ReadAllText(DataFile(expected)), outputContents);
            
        }
    }

    public static class Path
    {
        public static string DataFile(string relativePath) {
            return $"../../../{relativePath}";
        }
    }
}
