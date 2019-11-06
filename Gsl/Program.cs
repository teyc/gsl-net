using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Gsl
{
    class Program
    {
        static void Main(string[] args)
        {
            var vm = new VM(new FileSystem(), LoggerFactory.Create(b => b.AddConsole()).CreateLogger<Program>());

            var engine = new Jint.Engine(options =>
            {
                options.DebugMode(true);
            }).SetValue("log", new Action<object>(line => Console.WriteLine("log: " + line)))
              .SetValue("output", new Action<object>(vm.WriteLine))
              .SetValue("outputAligned", new Action<int, string>(vm.WriteLineAligned))
              .SetValue("setOutput", new Action<string>(vm.SetOutput))
              .SetValue("fields", new[] { "FirstName", "LastName", "DateOfBirth" });


            var template = File.ReadAllText("Gsl.Tests/data/align.gsl");

            var parser = new TemplateParser();
            var script = string.Join("\n", 
                template.Split("\n")
                    .Select(parser.TranslateLine));
            try
            {
                Console.WriteLine(script);
                engine.Execute(script);
            }
            catch (Exception exception)
            {
                Console.WriteLine(script);
                Console.Error.WriteLine(exception.Message);
                Console.Error.WriteLine(exception.StackTrace);
            }
        }
    }
}
