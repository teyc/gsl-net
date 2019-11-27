namespace Gsl.Handlers
{
    public class ProtectedHandler : IHandler
    {
        public (bool handled, string js) Handle(int lineNumber, string line)
        {
            return (false, "");
        }

        public void WriteTo(AddOutput addOutput, object[] args)
        {
            if (addOutput is null)
            {
                throw new System.ArgumentNullException(nameof(addOutput));
            }

            if (args is null)
            {
                throw new System.ArgumentNullException(nameof(args));
            }

            var sectionName = (string) args[0];
            var prefix = (string) args[1];
            var suffix = (string) args[2];
            addOutput(new OutputBuffer.ProtectedSection(sectionName, prefix, suffix));
        }
    }
}