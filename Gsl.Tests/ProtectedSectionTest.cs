using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Gsl.Tests
{
    public class ProtectedSectionTest : TestBase
    {
        public ProtectedSectionTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void ProtectedSectionsShouldPreserveUserContent()
        {
            const string template = "data/protected.gsl";
            const string data = "data/protected.json";

            _fileSystem.File.WriteAllText("Customer.cs", File.ReadAllText(Path.DataFile("data/Customer.cs")));
            TemplateWithSingleFileOutput(template, data);
        }
    }
}
