using System;
using System.Threading.Tasks;
using Android.Util;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Structured logging service that outputs to Android Logcat and optionally persists to SQLite.
    /// All log methods are synchronous for Logcat (non-blocking) with optional async database persistence.
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly ILogRepository? _logRepository;
        private readonly bool _loggingEnabled;

        /// <summary>
        /// Initializes a new instance of <see cref="LoggerService"/>.
        /// </summary>
        /// <param name="logRepository">The log repository for database persistence. Can be null during startup.</param>
        /// <param name="loggingEnabled">Whether database logging is enabled.</param>
        public LoggerService(ILogRepository? logRepository = null, bool loggingEnabled = true)
        {
            _logRepository = logRepository;
            _loggingEnabled = loggingEnabled;
        }

        /// <inheritdoc />
        public void Debug(string tag, string message)
        {
            Log.Debug(tag, message);
        }

        /// <inheritdoc />
        public void Info(string tag, string message)
        {
            Log.Info(tag, message);
        }

        /// <inheritdoc />
        public void Warning(string tag, string message)
        {
            Log.Warn(tag, message);
        }

        /// <inheritdoc />
        public void Error(string tag, string message, Exception? exception = null)
        {
            string fullMessage = exception != null
                ? $"{message} | Exception: {exception.GetType().Name}: {exception.Message}"
                : message;

            Log.Error(tag, fullMessage);

            if (_loggingEnabled && _logRepository != null)
            {
                LogToDbAsync(tag, message, "Error", exception).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public void Critical(string tag, string message, Exception? exception = null)
        {
            string fullMessage = exception != null
                ? $"[CRITICAL] {message} | Exception: {exception.GetType().Name}: {exception.Message}"
                : $"[CRITICAL] {message}";

            Log.Error(tag, fullMessage);

            if (_loggingEnabled && _logRepository != null)
            {
                LogToDbAsync(tag, message, "Critical", exception).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task LogToDbAsync(string source, string message, string level, Exception? exception = null)
        {
            if (_logRepository == null) return;

            try
            {
                var errorLog = new ErrorLogModel
                {
                    Source = source,
                    Message = message.Length > 1000 ? message.Substring(0, 1000) : message,
                    StackTrace = exception?.StackTrace?.Length > 4000
                        ? exception.StackTrace.Substring(0, 4000)
                        : exception?.StackTrace ?? string.Empty,
                    Level = level,
                    CreatedAt = DateTime.UtcNow
                };

                await _logRepository.InsertErrorLogAsync(errorLog);
            }
            catch (Exception ex)
            {
                // Avoid recursive logging failures — just write to Logcat
                Log.Error("LoggerService", $"Failed to persist log to DB: {ex.Message}");
            }
        }
    }
}
