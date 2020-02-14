using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Gsl.Tests
{
    public class IncludeTest : TestBase
    {
        public IncludeTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void An_include_directive_should_include_based_on_relative_path()
        {
            const string template = "data/include-1.gsl";
            const string data = "data/include.json";

            var include2Path = new FileInfo(Path.DataFile("data\\include-2.gsl")).FullName;
            var include2Directory = _fileSystem.Path.GetDirectoryName(include2Path);
            _fileSystem.Directory.CreateDirectory(include2Directory);
            _fileSystem.File.WriteAllText(
                include2Path,
                File.ReadAllText(include2Path));

            TemplateWithSingleFileOutput(template, data);
        }
    }
}
