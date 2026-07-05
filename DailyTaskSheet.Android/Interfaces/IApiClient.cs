using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Generic API client interface for making authenticated HTTP requests.
    /// Handles serialization, JWT injection, retry, and error mapping.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>Sends an authenticated GET request and deserializes the response.</summary>
        /// <typeparam name="T">The expected response data type.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The API result containing the response data.</returns>
        Task<ApiResult<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>Sends an authenticated POST request with a JSON body.</summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response data type.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="request">The request body object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The API result containing the response data.</returns>
        Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);

        /// <summary>Sends an unauthenticated POST request (used for login).</summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response data type.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="request">The request body object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The API result containing the response data.</returns>
        Task<ApiResult<TResponse>> PostAnonymousAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);

        /// <summary>Sends an authenticated PUT request with a JSON body.</summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response data type.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="request">The request body object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The API result containing the response data.</returns>
        Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);

        /// <summary>Sends an authenticated DELETE request.</summary>
        /// <typeparam name="T">The expected response data type.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The API result containing the response data.</returns>
        Task<ApiResult<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>Sends an authenticated multipart/form-data POST request to upload a file.</summary>
        /// <typeparam name="TResponse">The expected response data type.</typeparam>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="filePath">The local file path to upload.</param>
        /// <param name="fileParameterName">The parameter name expected by the server (e.g. "file").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The API result containing the response data.</returns>
        Task<ApiResult<TResponse>> UploadFileAsync<TResponse>(string endpoint, string filePath, string fileParameterName = "file", CancellationToken cancellationToken = default);

        /// <summary>Sets the base URL for all API requests.</summary>
        /// <param name="baseUrl">The base URL (e.g., "https://api.dailytasksheet.com").</param>
        void SetBaseUrl(string baseUrl);
    }
}
