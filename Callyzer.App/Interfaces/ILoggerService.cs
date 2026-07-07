namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Interface for logging application events, warnings, and errors.
    /// </summary>
    public interface ILoggerService
    {
        void Debug(string tag, string message);
        void Info(string tag, string message);
        void Warning(string tag, string message);
        void Error(string tag, string message, System.Exception? exception = null);
    }
}
