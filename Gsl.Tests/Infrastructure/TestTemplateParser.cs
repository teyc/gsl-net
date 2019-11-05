using Xunit;

namespace Gsl.Tests.Infrastructure
{
    public class TestTemplateParser
    {
        [Fact]
        public void TestParseAlignment()
        {
            var parser = new TemplateParser();
            var lines = new [] {
                ".    |    |",
                "abcdefghijkl"
            };
            parser.TranslateLine(lines[0]);
            var tokens = parser.ParseInterpolatedStringWithAlignment(1, lines[1]);
            Assert.Equal(new [] {
                new StringToken("abcde"),
                new StringToken("\0"),
                new StringToken("fghij"),
                new StringToken("\0"),
                new StringToken("kl"),
            }, tokens);
        }
    }
}