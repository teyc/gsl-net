using Jint.Native.Json;
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
            if (templateFile is null)
            {
                throw new ArgumentNullException(nameof(templateFile));
            }

            if (dataFile is null)
            {
                throw new ArgumentNullException(nameof(dataFile));
            }

            var template = templateFile.OpenText().ReadToEnd();
            var dataContents = dataFile.OpenText().ReadToEnd();

            return Execute(template, dataContents);
        }

        private IFileInfo[] Execute(string template, string dataContents)
        {
            var parser = new TemplateParser();
            var script = string.Join("\n",
                template.Split("\n")
                    .Select(parser.TranslateLine));

            var jsEngine = new Jint.Engine(options => options.DebugMode())
              .SetValue("log", new Action<object>(line => Console.WriteLine("log: " + line)))
              .SetValue("output", new Action<object>(vm.WriteLine))
              .SetValue("outputAligned", new Action<int, string>(vm.WriteLineAligned))
              .SetValue("protect", new Action<string, string, string>(vm.WriteProtectedSection))
              .SetValue("doNotOverwriteIf", new Action<string, string>(vm.DoNotOverwriteIf))
              .SetValue("setOutput", new Action<string>(vm.SetOutput));

            var data = new JsonParser(jsEngine).Parse(dataContents);
            foreach (var property in data.AsObject().GetOwnProperties())
            {
                jsEngine.Global.Put(property.Key, property.Value.Value, true);
            }

            jsEngine.Execute(script);

            return vm.GetOutputFiles();
        }
    }
}