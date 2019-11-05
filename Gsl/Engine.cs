using System;
using System.IO.Abstractions;
using System.Linq;

namespace Gsl
{
    public class Engine
    {
        private readonly VM vm;

        public Engine(VM vm)
        {
            this.vm = vm ?? throw new ArgumentNullException(nameof(vm));
        }
        
        public IFileInfo[] Execute(IFileInfo templateFile, IFileInfo dataFile)
        {
            var template = templateFile.OpenText().ReadToEnd();

            var parser = new TemplateParser();
            var script = string.Join("\n", 
                template.Split("\n")
                    .Select(parser.TranslateLine));

            var jsEngine = new Jint.Engine(options => options.DebugMode())
              .SetValue("log", new Action<object>(line => Console.WriteLine("log: " + line)))
              .SetValue("output", new Action<object>(vm.WriteLine))
              .SetValue("outputAligned", new Action<int, string>(vm.WriteLineAligned))
              .SetValue("setOutput", new Action<string>(vm.SetOutput))
              .SetValue("fields", new[] { "FirstName", "LastName", "DateOfBirth" });

            jsEngine.Execute(script);

            return vm.GetOutputFiles();
        }
    }
}