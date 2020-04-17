using Jint.Native.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;

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

            return Execute(template, dataContents, templateFile.FullName);
        }

        private IFileInfo[] Execute(string template, string dataContents, string templatePath)
        {
            var jsEngine = BuildJintEngine();

            var data = new JsonParser(jsEngine).Parse(dataContents);
            foreach (var property in data.AsObject().GetOwnProperties())
            {
                jsEngine.Global.Put(property.Key, property.Value.Value, true);
            }

            vm.EvaluateTemplate(jsEngine: jsEngine, templatePath: templatePath, templateContent: template);

            return vm.GetOutputFiles();
        }

        public IFileInfo[] Execute(string template, IDictionary<string, object> data, string templatePath)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Jint.Engine jsEngine = BuildJintEngine();

            foreach (var property in data)
            {
                jsEngine.SetValue(property.Key, property.Value);
            }

            vm.EvaluateTemplate(jsEngine: jsEngine, templatePath: templatePath, templateContent: template);

            return vm.GetOutputFiles();
        }

        private Jint.Engine BuildJintEngine()
        {
            var jsEngine = new Jint.Engine(options =>
            {
                options.AllowClr();
                options.DebugMode();
            })
              .SetValue("__expandText", new Func<string, string>(vm.ExpandText))
              .SetValue("__optionalText", new Func<string, string>(vm.OptionalText))
              .SetValue("replaceText", new Action<string, string>(vm.ReplaceText))
              .SetValue("log", new Action<object>(line => logger.LogInformation("log: " + line)))
              .SetValue("kebabCase", new Func<string, string>(StringFunctions.KebabCase))
              .SetValue("camelCase", new Func<string, string>(StringFunctions.CamelCase))
              .SetValue("output", new Action<object>(vm.WriteLine))
              .SetValue("outputAligned", new Action<int, string>(vm.WriteLineAligned))
              .SetValue("protect", new Action<string, string, string>(vm.WriteProtectedSection))
              .SetValue("doNotOverwriteIf", new Action<string, string>(vm.DoNotOverwriteIf))
              .SetValue("setOutput", new Action<string>(vm.SetOutput));

            jsEngine
              .SetValue("include", new Action<string>(
                  relativePath => vm.EvaluateTemplate(jsEngine: jsEngine, templatePath: relativePath)));

            return jsEngine;
        }
    }
}