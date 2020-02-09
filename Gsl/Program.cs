using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Gsl
{
    class Program
    {
        private static void ShowHelp()
        {
            Console.Out.WriteLine("Gsl.exe <pathToTempate.gsl> <pathToData.json>");
        }

        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                ShowHelp();
                return;
            }

            var pathToTemplate = args[0];
            var pathToData = args[1];

            using var loggerFactory = LoggerFactory.Create(b =>
            {
                b.AddConsole();
                //b.SetMinimumLevel(LogLevel.Trace);
            });
            var logger = loggerFactory.CreateLogger<Program>();
            var fileSystem = new FileSystem();
            var engine = new Gsl.Engine(new Gsl.VM(fileSystem, logger));
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
