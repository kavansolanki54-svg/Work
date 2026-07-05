using System;
using System.Threading.Tasks;
using Android.Util;

namespace DailyTaskSheet.App.Extensions
{
    /// <summary>
    /// Extension methods for safe async task execution (fire-and-forget pattern).
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Safely executes a task without awaiting, logging any exceptions.
        /// Use this for fire-and-forget scenarios (e.g., logging, notifications)
        /// where you don't want to block the calling thread.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="tag">Log tag for error reporting.</param>
        /// <param name="onException">Optional exception handler.</param>
        public static async void SafeFireAndForget(
            this Task task,
            string tag = "TaskExtension",
            Action<Exception>? onException = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                Log.Error(tag, $"SafeFireAndForget caught exception: {ex.Message}");
                onException?.Invoke(ex);
            }
        }

        /// <summary>
        /// Executes a task with a timeout.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="task">The task to execute.</param>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The task result, or throws TimeoutException.</returns>
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));
            if (completedTask == task)
            {
                return await task;
            }
            throw new TimeoutException($"Task timed out after {timeout.TotalSeconds} seconds.");
        }

        /// <summary>
        /// Executes a task with a timeout (void version).
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="timeout">The timeout duration.</param>
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));
            if (completedTask != task)
            {
                throw new TimeoutException($"Task timed out after {timeout.TotalSeconds} seconds.");
            }
            await task; // Propagate any exceptions
        }
    }
}
