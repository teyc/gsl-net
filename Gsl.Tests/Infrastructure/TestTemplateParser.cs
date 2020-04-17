using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Gsl.Tests.Infrastructure
{
    public class TestTemplateParser
    {
        private readonly ITestOutputHelper output;

        public TestTemplateParser(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public void TestParseAlignment()
        {
            ILogger<TestTemplateParser> logger = CreateLogger();
            var handler = new Gsl.Handlers.AlignHandler(logger);
            var lines = new[] {
                ".    |    |",
                "abcdefghijkl"
            };
            var (ok, handled) = handler.Handle(1, lines[0]);
            Assert.True(ok);
            var tokens = handler.ParseInterpolatedStringWithAlignment(1, lines[1]);
            Assert.Equal(new[] {
                new StringToken("abcde"),
                StringToken.ALIGN_LEFT,
                new StringToken("fghij"),
                StringToken.ALIGN_LEFT,
                new StringToken("kl"),
            }, tokens);
        }

        [Fact]
        public void TestParseOptional()
        {
            ILogger<TestTemplateParser> logger = CreateLogger();
            var handler = new Gsl.Handlers.AlignHandler(logger);
            var lines = new[] {
                ".    |    ?",
                "abcdefghijkl"
            };
            var (ok, _) = handler.Handle(1, lines[0]);
            Assert.True(ok);
            var tokens = handler.ParseInterpolatedStringWithAlignment(1, lines[1]);
            Assert.Equal(new[] {
                new StringToken("abcde"),
                StringToken.ALIGN_LEFT,
                new StringToken("fghij"),
                StringToken.OPTIONAL,
                new OptionalToken("kl"),
            }, tokens);
        }

        private ILogger<TestTemplateParser> CreateLogger()
        {
            return LoggerFactory.Create(configure =>
                            configure.SetMinimumLevel(LogLevel.Trace)
                            .AddProvider(new XUnitLogProvider(output)))
                            .CreateLogger<TestTemplateParser>();
        }
    }
}