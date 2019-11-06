using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Gsl
{
    class Program
    {
        static void ShowHelp()
        {

        }

        static void Main(string[] args)
        {
            if (args.Length == 0) 
            {
                ShowHelp();
                return;
            }
            
            var pathToTemplate = args[0];
            var pathToData = args[1];

            using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
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
