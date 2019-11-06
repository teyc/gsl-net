namespace Gsl.Handlers
{
    public interface IHandler
    {
        (bool handled, string js) Handle(int lineNumber, string line);
    }
}