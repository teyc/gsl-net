using static System.StringComparison;

namespace Gsl.Handlers
{
    public class JsHandler : IHandler
    {
        public (bool handled, string js) Handle(int lineNumber, string line)
        {
            if (line is null)
            {
                throw new System.ArgumentNullException(nameof(line));
            }

            if (line.StartsWith(". ", InvariantCulture))
            {
                return (true, line.Substring(2));
            }

            return (false, "");
        }

        public void WriteTo(AddOutput addOutput, object[] args)
        {
        }
    }
}
