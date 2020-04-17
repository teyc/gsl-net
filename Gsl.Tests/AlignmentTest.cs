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

        [Fact]
        public void NestedAlignProperties()
        {
            const string template = "data/align2.gsl";
            const string data = "data/align2.json";

            TemplateWithSingleFileOutput(template, data);
        }

        [Fact]
        public void Optional()
        {
            const string template = "data/optional.gsl";
            const string data = "data/optional.json";

            TemplateWithSingleFileOutput(template, data);
        }
    }
}
