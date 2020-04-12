namespace Gsl.Handlers
{
    public delegate void AddOutput(object token);

    public interface IHandler
    {
        (bool handled, string js) Handle(int lineNumber, string line);

        void WriteTo(AddOutput addOutput, object[] args);
    }
}