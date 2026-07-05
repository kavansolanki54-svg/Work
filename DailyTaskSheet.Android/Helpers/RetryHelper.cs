using System;
using System.Threading;
using System.Threading.Tasks;

namespace DailyTaskSheet.App.Helpers
{
    /// <summary>
    /// Provides exponential backoff retry logic for transient failures.
    /// Used by the sync service, API client, and background workers.
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// Executes an async operation with exponential backoff retry.
        /// </summary>
        /// <typeparam name="T">The return type of the operation.</typeparam>
        /// <param name="operation">The async operation to execute.</param>
        /// <param name="maxRetries">Maximum number of retry attempts.</param>
        /// <param name="initialDelayMs">Initial delay in milliseconds before the first retry.</param>
        /// <param name="maxDelayMs">Maximum delay in milliseconds between retries.</param>
        /// <param name="shouldRetry">Predicate to determine if the exception is retryable. Defaults to always retry.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the successful operation.</returns>
        /// <exception cref="AggregateException">Thrown when all retries are exhausted.</exception>
        public static async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            int maxRetries = 3,
            int initialDelayMs = 1000,
            int maxDelayMs = 30000,
            Func<Exception, bool>? shouldRetry = null,
            CancellationToken cancellationToken = default)
        {
            shouldRetry ??= _ => true;
            Exception? lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await operation();
                }
                catch (global::System.OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    if (attempt >= maxRetries || !shouldRetry(ex))
                    {
                        throw;
                    }

                    int delay = CalculateDelay(attempt, initialDelayMs, maxDelayMs);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw lastException ?? new InvalidOperationException("Retry exhausted with no exception captured.");
        }

        /// <summary>
        /// Executes an async void operation with exponential backoff retry.
        /// </summary>
        /// <param name="operation">The async operation to execute.</param>
        /// <param name="maxRetries">Maximum number of retry attempts.</param>
        /// <param name="initialDelayMs">Initial delay in milliseconds.</param>
        /// <param name="maxDelayMs">Maximum delay in milliseconds.</param>
        /// <param name="shouldRetry">Predicate for retryable exceptions.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task ExecuteWithRetryAsync(
            Func<Task> operation,
            int maxRetries = 3,
            int initialDelayMs = 1000,
            int maxDelayMs = 30000,
            Func<Exception, bool>? shouldRetry = null,
            CancellationToken cancellationToken = default)
        {
            await ExecuteWithRetryAsync(async () =>
            {
                await operation();
                return true;
            }, maxRetries, initialDelayMs, maxDelayMs, shouldRetry, cancellationToken);
        }

        /// <summary>
        /// Calculates the exponential backoff delay with jitter.
        /// </summary>
        /// <param name="attempt">The current attempt number (0-based).</param>
        /// <param name="initialDelayMs">Initial base delay in milliseconds.</param>
        /// <param name="maxDelayMs">Maximum delay cap in milliseconds.</param>
        /// <returns>The calculated delay in milliseconds.</returns>
        public static int CalculateDelay(int attempt, int initialDelayMs = 1000, int maxDelayMs = 30000)
        {
            // Exponential: initialDelay * 2^attempt
            double exponentialDelay = initialDelayMs * Math.Pow(2, attempt);

            // Add jitter (±25% randomization to prevent thundering herd)
            var random = new Random();
            double jitter = exponentialDelay * 0.25 * (random.NextDouble() * 2 - 1);

            int delay = (int)Math.Min(exponentialDelay + jitter, maxDelayMs);
            return Math.Max(delay, initialDelayMs);
        }

        /// <summary>
        /// Calculates the next retry time based on the current attempt.
        /// </summary>
        /// <param name="attempt">The current attempt number.</param>
        /// <param name="initialDelaySeconds">Initial delay in seconds.</param>
        /// <returns>The DateTime for the next retry attempt.</returns>
        public static DateTime CalculateNextRetryTime(int attempt, int initialDelaySeconds = 30)
        {
            int delayMs = CalculateDelay(attempt, initialDelaySeconds * 1000, 3600000); // Max 1 hour
            return DateTime.UtcNow.AddMilliseconds(delayMs);
        }
    }
}
