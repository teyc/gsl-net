using Xunit;

namespace Gsl.Tests
{
    public class ReplaceTextTest : TestBase
    {
        public ReplaceTextTest(Xunit.Abstractions.ITestOutputHelper log) : base(log)
        {
        }

        /// <summary>
        /// All raw text are macro expanded when replaceText definition is found
        /// </summary>
        [Fact]
        public void TemplateTextShouldBeReplaced()
        {
            const string template = "data/replaceText.gsl";
            const string data = "data/replaceText.json";

            TemplateWithMultipleFileOutput(template, data);
        }
    }
}