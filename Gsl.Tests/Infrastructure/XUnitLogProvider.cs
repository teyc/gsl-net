using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Gsl.Tests.Infrastructure
{
    public class XUnitLogProvider : ILoggerProvider, ILogger
    {
        private readonly ITestOutputHelper output;

        public XUnitLogProvider(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            output.WriteLine(formatter(state, exception));
        }
    }
}