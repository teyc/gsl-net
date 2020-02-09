using Jint.Native.Json;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Abstractions;
using System.Linq;

namespace Gsl
{
    public class Engine
    {
        private readonly VM vm;
        private readonly ILogger logger;

        public Engine(VM vm, ILogger logger)
        {
            this.vm = vm ?? throw new ArgumentNullException(nameof(vm));
            this.logger = logger;
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
            var jsEngine = new Jint.Engine(options => options.DebugMode())
              .SetValue("log", new Action<object>(line => logger.LogInformation("log: " + line)))
              .SetValue("kebabCase", new Func<string, string>(StringFunctions.KebabCase))
              .SetValue("camelCase", new Func<string, string>(StringFunctions.CamelCase))
              .SetValue("doNotOverwriteIf", new Action<string, string>(vm.DoNotOverwriteIf));

            var data = new JsonParser(jsEngine).Parse(dataContents);
            foreach (var property in data.AsObject().GetOwnProperties())
            {
                jsEngine.Global.Put(property.Key, property.Value.Value, true);
            }

            /* run a zero-output pass - to gather replace text */
            var parser = new TemplateParser(new Handlers.AlignHandler(logger), vm.ReplaceTextPreprocessor);
            var script = string.Join("\n",
                template.Split("\n")
                    .Select(parser.TranslateLine));
            logger.LogTrace("script: {script}", script);

            jsEngine
                .SetValue("setOutput", new Action<string>(_ => { }))
                .SetValue("output", new Action<object>(_ => { }))
                .SetValue("outputAligned", new Action<int, string>((_, dummy) => { }))
                .SetValue("protect", new Action<string, string, string>((_, dummy, dummy2) => { }))
                .SetValue("replaceText", new Action<string, string>(vm.ReplaceText))
                .Execute(script);

            /* the parser's replaceText engine has been populated, uuuggly */
            script = string.Join("\n",
                            template.Split("\n")
                                .Select(parser.TranslateLine));

            jsEngine
                .SetValue("setOutput", new Action<string>(vm.SetOutput))
                .SetValue("output", new Action<object>(vm.WriteLine))
                .SetValue("outputAligned", new Action<int, string>(vm.WriteLineAligned))
                .SetValue("protect", new Action<string, string, string>(vm.WriteProtectedSection))
                .SetValue("replaceText", new Action<string, string>((_, dummy) => { }))
                .Execute(script);

            return vm.GetOutputFiles();
        }
    }
}