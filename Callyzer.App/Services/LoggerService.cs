using Callyzer.App.Interfaces;

namespace Callyzer.App.Services
{
    /// <summary>
    /// Simple debug logger implementation using System.Diagnostics.Debug.
    /// Writes to IDE output window during development.
    /// </summary>
    public class LoggerService : ILoggerService
    {
        public void Debug(string tag, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] [{tag}] {message}");
        }

        public void Info(string tag, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [{tag}] {message}");
        }

        public void Warning(string tag, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARN] [{tag}] {message}");
        }

        public void Error(string tag, string message, Exception? exception = null)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] [{tag}] {message}");
            if (exception != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] [{tag}] Exception: {exception}");
            }
        }
    }
}
