using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gsl.Handlers
{
    public class ReplaceTextPreprocessor: IHandler
    {
        private readonly List<KeyValuePair<string, string>> _replacements = new List<KeyValuePair<string, string>>();

        public void Add(string search, string replace)
        {
            _replacements.Add(new KeyValuePair<string, string>(search, replace));
        }

        public string Expand(string input)
        {
            var output = input;
            foreach (var r in _replacements)
            {
                output = output.Replace(r.Key, r.Value, StringComparison.InvariantCulture);
            }
            return output;
        }

        public (bool handled, string js) Handle(int lineNumber, string line)
        {
            return (false, null);
        }

        public void WriteTo(AddOutput addOutput, object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}