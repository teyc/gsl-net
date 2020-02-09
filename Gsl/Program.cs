using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Gsl
{
    internal class Program
    {
        private static void ShowHelp()
        {
            Console.Out.WriteLine("Gsl.exe <pathToTempate.gsl> <pathToData.json> [--verbose]");
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowHelp();
                return;
            }

            var pathToTemplate = args[0];
            var pathToData = args[1];
            var verbose = args.Skip(2).Any(arg => arg == "--verbose");

            using var loggerFactory = LoggerFactory.Create(b =>
            {
                b.AddConsole();
                if (verbose) b.SetMinimumLevel(LogLevel.Trace);
            });
            var logger = loggerFactory.CreateLogger<Program>();
            var fileSystem = new FileSystem();
            var engine = new Engine(new Gsl.VM(fileSystem, logger), logger);
            try
            {
                engine.Execute(new FileInfoWrapper(fileSystem, new FileInfo(pathToTemplate)),
                               new FileInfoWrapper(fileSystem, new FileInfo(pathToData)));
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Console.Error.WriteLine(exception.StackTrace);
            }
        }
    }
}
