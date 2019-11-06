using System.IO;
using System.IO.Abstractions;
using System.Linq;
using ApprovalTests;
using Xunit;
using Xunit.Abstractions;
using static Gsl.Tests.Path;

namespace Gsl.Tests
{

    public class AlignmentTest: TestBase
    {
        public AlignmentTest(ITestOutputHelper log): base(log) { }

        [Fact]
        public void AlignProperties()
        {
            const string template = "data/align.gsl";
            const string data = "data/align.json";

            var outputFiles = _gslEngine.Execute(
                new FileInfoWrapper(_fileSystem, new FileInfo(DataFile(template))),
                new FileInfoWrapper(_fileSystem, new FileInfo(DataFile(data))));

            outputFiles.ToList().ForEach(f => _log.WriteLine(f.Name));
            var outputFile = outputFiles[0];
            var outputContents = outputFile.ReadToEnd();

            Approvals.Verify(outputContents);
        }
    }
}
