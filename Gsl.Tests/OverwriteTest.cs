using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Gsl.Tests
{
    public class OverwriteTest : TestBase
    {
        public OverwriteTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void DoNotOverwriteFilesIfSpecialTextIsPresent()
        {
            const string template = "data/overwrite.gsl";
            const string data = "data/overwrite.json";

            // Given that an output file already exists, we don't want to overwrite it
            _fileSystem.File.WriteAllText("Overwrite.cs",
                File.ReadAllText(Path.DataFile("data/OverwriteTest.DoNotOverwriteFilesIfSpecialTextIsPresent.approved.txt")));

            TemplateWithSingleFileOutput(template, data);
        }
    }
}
