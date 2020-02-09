using Xunit;

namespace Gsl.Tests
{
    public class ReplaceTextTest : TestBase
    {
        public ReplaceTextTest(Xunit.Abstractions.ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void TemplateTextShouldBeReplaced()
        {
            const string template = "data/replaceText.gsl";
            const string data = "data/replaceText.json";

            TemplateWithSingleFileOutput(template, data);
        }
    }
}