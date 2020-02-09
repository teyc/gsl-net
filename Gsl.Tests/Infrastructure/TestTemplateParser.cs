using Microsoft.Extensions.Logging;
using Xunit;

namespace Gsl.Tests.Infrastructure
{
    public class TestTemplateParser
    {
        [Fact]
        public void TestParseAlignment()
        {
            var logger = LoggerFactory.Create(configure =>
                configure.SetMinimumLevel(LogLevel.Trace).AddConsole())
                .CreateLogger<TestTemplateParser>();
            var handler = new Gsl.Handlers.AlignHandler(logger);
            var lines = new [] {
                ".    |    |",
                "abcdefghijkl"
            };
            var (ok, handled) = handler.Handle(1, lines[0]);
            Assert.True(ok);
            var tokens = handler.ParseInterpolatedStringWithAlignment(1, lines[1]);
            Assert.Equal(new [] {
                new StringToken("abcde"),
                StringToken.NULL,
                new StringToken("fghij"),
                StringToken.NULL,
                new StringToken("kl"),
            }, tokens);
        }
    }
}