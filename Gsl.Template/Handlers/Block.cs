using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using static System.StringComparison;

namespace Gsl.Handlers
{
    public class Block
    {
        protected Block() { }

        public int Width { get; private set; }
        public bool IsOptional { get; private set; }

        public static Block CreateAlignmentBlock(int width) { return new Block { Width = width }; }

        public static Block CreateOptionalBlock(int width) { return new Block { Width = width, IsOptional = true }; }
    }
}