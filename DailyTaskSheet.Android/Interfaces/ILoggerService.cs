using System;
using System.Threading.Tasks;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for structured application logging.
    /// Logs to both Android Logcat and SQLite ErrorLogs table.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>Logs a debug-level message.</summary>
        void Debug(string tag, string message);

        /// <summary>Logs an informational message.</summary>
        void Info(string tag, string message);

        /// <summary>Logs a warning message.</summary>
        void Warning(string tag, string message);

        /// <summary>Logs an error message.</summary>
        void Error(string tag, string message, Exception? exception = null);

        /// <summary>Logs a critical error message.</summary>
        void Critical(string tag, string message, Exception? exception = null);

        /// <summary>Logs an error asynchronously to the SQLite database.</summary>
        Task LogToDbAsync(string source, string message, string level, Exception? exception = null);
    }
}
