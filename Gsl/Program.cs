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
            Console.Out.WriteLine(@"Gsl.exe <pathToTempate.gsl> <pathToData.json> [--verbose]

A gsl program consists of lines that begin with '.' (period) which is generally interpreted with a ES5
javascript engine, and lines which do not - is output as javascript template strings.

Example:

    . var name = 'World';
    Hello ${name}!

outputs:

    Hello World!

Functions:

    output(text)                          - writes text to the output
    protect(sectionName, prefix, suffix)  - creates a protected section
    doNotOverwriteIf(incantation, alternateExtension)
                                          - if a piece of text is present in the output file, 
                                            output is written to a file with an alternate file 
                                            extension
    setOutput(filename)                   - set the output filename (relative to working directory)
    replaceText(search, replace)          - replaces text in the template with an alternative
    log(message)                          - for printf-style debugging

    kebabCase(properCase)                 - turns WikiCase to wiki-case
    camelCase(properCase)                 - turns WikiCase to wikiCase
    include(filename)                     - includes a file relative to current file
");
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
