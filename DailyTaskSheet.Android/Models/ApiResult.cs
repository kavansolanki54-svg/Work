using System;
using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Generic API result wrapper matching the backend's ApiResponse&lt;T&gt; structure.
    /// Used for all REST API communications.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class ApiResult<T>
    {
        /// <summary>Gets or sets whether the API operation was successful.</summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>Gets or sets the human-readable message from the API.</summary>
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>Gets or sets the data payload returned by the API.</summary>
        [JsonProperty("data")]
        public T? Data { get; set; }

        /// <summary>Gets or sets validation or error details keyed by field name.</summary>
        [JsonProperty("errors")]
        public System.Collections.Generic.Dictionary<string, string[]>? Errors { get; set; }

        /// <summary>
        /// Creates a successful local result (not from API).
        /// </summary>
        public static ApiResult<T> SuccessResult(T? data, string message = "")
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed local result (not from API).
        /// </summary>
        public static ApiResult<T> ErrorResult(string message)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }
}
