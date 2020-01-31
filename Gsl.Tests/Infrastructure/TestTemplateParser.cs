using Xunit;

namespace Gsl.Tests.Infrastructure
{
    public class TestTemplateParser
    {
        [Fact]
        public void TestParseAlignment()
        {
            var handler = new Gsl.Handlers.AlignHandler();
            var lines = new [] {
                ".    |    |",
                "abcdefghijkl"
            };
            var (ok, handled) = handler.Handle(1, lines[0]);
            Assert.True(ok);
            var tokens = handler.ParseInterpolatedStringWithAlignment(1, lines[1]);
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