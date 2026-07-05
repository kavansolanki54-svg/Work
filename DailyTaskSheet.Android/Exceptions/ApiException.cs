using System;
using System.Net;

namespace DailyTaskSheet.App.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when an API request fails.
    /// Contains the HTTP status code and response body for diagnostic purposes.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>Gets the HTTP status code returned by the API.</summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>Gets the raw response body from the API.</summary>
        public string? ResponseBody { get; }

        /// <summary>Gets the API endpoint that was called.</summary>
        public string? Endpoint { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiException"/>.
        /// </summary>
        /// <param name="message">Human-readable error message.</param>
        /// <param name="statusCode">HTTP status code from the response.</param>
        /// <param name="responseBody">Raw response body for diagnostics.</param>
        /// <param name="endpoint">The API endpoint that was called.</param>
        /// <param name="innerException">Optional inner exception.</param>
        public ApiException(
            string message,
            HttpStatusCode statusCode,
            string? responseBody = null,
            string? endpoint = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            Endpoint = endpoint;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiException"/> for network-level failures.
        /// </summary>
        /// <param name="message">Human-readable error message.</param>
        /// <param name="innerException">The underlying network exception.</param>
        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Returns whether this exception represents a transient failure that can be retried.
        /// </summary>
        public bool IsTransient =>
            StatusCode == HttpStatusCode.RequestTimeout ||
            StatusCode == HttpStatusCode.TooManyRequests ||
            StatusCode == HttpStatusCode.InternalServerError ||
            StatusCode == HttpStatusCode.BadGateway ||
            StatusCode == HttpStatusCode.ServiceUnavailable ||
            StatusCode == HttpStatusCode.GatewayTimeout;

        /// <summary>
        /// Returns whether this exception indicates the authentication token has expired.
        /// </summary>
        public bool IsAuthenticationError =>
            StatusCode == HttpStatusCode.Unauthorized ||
            StatusCode == HttpStatusCode.Forbidden;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ApiException: {Message} | Status: {(int)StatusCode} ({StatusCode}) | Endpoint: {Endpoint ?? "N/A"}";
        }
    }
}
