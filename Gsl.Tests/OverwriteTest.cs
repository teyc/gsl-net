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

            // Arrange: Given that an output file already exists
            _fileSystem.File.WriteAllText("Overwrite.cs",
                File.ReadAllText(Path.DataFile("data/OverwriteTest.DoNotOverwriteFilesIfSpecialTextIsPresent.approved.txt")));

            // Act:
            TemplateWithSingleFileOutput(template, data);

            // Assert
            Assert.True(_fileSystem.File.Exists("Overwrite.cs.bak"), "Overwrite protection is on, and output is written elsewhere");
            Assert.Contains("THIS_FIELD_SHOULD_NOT_BE_PRESENT_BECAUSE_OVERWRITE_IS_ON", _fileSystem.File.ReadAllText("Overwrite.cs.bak"));
        }
    }
}
