using System;
using System.Collections.Generic;

namespace Gsl.Handlers
{
    public class ReplaceTextHandler
    {
        private readonly List<(string Search, string Replace)> _defs = new List<(string Search, string Replace)>();

        public string Replace(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            string output = input;
            foreach (var (search, replace) in _defs)
            {
                output = output.Replace(search, replace);
            }
            return output;
        }

        internal void Set(string search, string replace)
        {
            var index = _defs.FindIndex(kv => kv.Search == search);
            if (index != -1)
            {
                _defs[index] = (search, replace);
            }
            else
            {
                _defs.Add((search, replace));
            }
        }
    }
}