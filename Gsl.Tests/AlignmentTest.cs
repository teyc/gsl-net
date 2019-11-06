using Xunit;
using Xunit.Abstractions;

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

            TemplateWithSingleFileOutput(template, data);
        }

    }
}
