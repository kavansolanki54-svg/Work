using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Exceptions;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;
using Newtonsoft.Json;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Generic REST API client that handles JSON serialization, JWT authentication,
    /// automatic token refresh, retry logic, and comprehensive error mapping.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IPreferenceService _preferenceService;
        private readonly ILoggerService _logger;
        private readonly ILogRepository _logRepository;
        private string _baseUrl;
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss"
        };

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClient"/>.
        /// </summary>
        public ApiClient(
            HttpClient httpClient,
            IPreferenceService preferenceService,
            ILoggerService logger,
            ILogRepository logRepository)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _preferenceService = preferenceService;
            _logger = logger;
            _logRepository = logRepository;

            _baseUrl = _preferenceService.GetString(AppConstants.PrefKeyApiBaseUrl, AppConstants.DefaultApiBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(AppConstants.HttpTimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }

        /// <inheritdoc />
        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _preferenceService.SetString(AppConstants.PrefKeyApiBaseUrl, _baseUrl);
            _logger.Info("ApiClient", $"Base URL set to: {_baseUrl}");
        }

        /// <inheritdoc />
        public async Task<ApiResult<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            return await SendAsync<T>(HttpMethod.Get, endpoint, null, true, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(request, JsonSettings);
            return await SendAsync<TResponse>(HttpMethod.Post, endpoint, json, true, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ApiResult<TResponse>> PostAnonymousAsync<TRequest, TResponse>(
            string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(request, JsonSettings);
            return await SendAsync<TResponse>(HttpMethod.Post, endpoint, json, false, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
            string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(request, JsonSettings);
            return await SendAsync<TResponse>(HttpMethod.Put, endpoint, json, true, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ApiResult<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            return await SendAsync<T>(HttpMethod.Delete, endpoint, null, true, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request with optional authentication, logging, and error handling.
        /// </summary>
        private async Task<ApiResult<T>> SendAsync<T>(
            HttpMethod method, string endpoint, string? jsonBody,
            bool authenticate, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            string url = $"{_baseUrl}{endpoint}";
            string responseBody = string.Empty;
            int statusCode = 0;

            try
            {
                using var request = new HttpRequestMessage(method, url);

                if (authenticate)
                {
                    string? token = _preferenceService.GetString(AppConstants.PrefKeyAccessToken);
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }

                if (jsonBody != null)
                {
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                stopwatch.Stop();

                statusCode = (int)response.StatusCode;
                responseBody = await response.Content.ReadAsStringAsync();

                // Log the API call
                await LogApiCallAsync(endpoint, method.Method, statusCode, jsonBody, responseBody, stopwatch.ElapsedMilliseconds, response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResult<T>>(responseBody, JsonSettings);
                    return result ?? ApiResult<T>.ErrorResult("Empty response from server.");
                }

                // Handle specific error codes
                return HandleErrorResponse<T>(response.StatusCode, responseBody, endpoint);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                _logger.Error("ApiClient", $"Request timeout for {method} {endpoint}", ex);
                throw new ApiException("Request timed out.", HttpStatusCode.RequestTimeout, endpoint: endpoint);
            }
            catch (global::System.OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                _logger.Error("ApiClient", $"HTTP error for {method} {endpoint}", ex);
                await LogApiCallAsync(endpoint, method.Method, 0, jsonBody, ex.Message, stopwatch.ElapsedMilliseconds, false);
                throw new ApiException($"Network error: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                _logger.Error("ApiClient", $"JSON parse error for {method} {endpoint}", ex);
                return ApiResult<T>.ErrorResult($"Invalid server response: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps HTTP error status codes to appropriate ApiResult error responses.
        /// </summary>
        private ApiResult<T> HandleErrorResponse<T>(HttpStatusCode statusCode, string responseBody, string endpoint)
        {
            string message = statusCode switch
            {
                HttpStatusCode.Unauthorized => "Session expired. Please log in again.",
                HttpStatusCode.Forbidden => "You do not have permission for this action.",
                HttpStatusCode.NotFound => "The requested resource was not found.",
                HttpStatusCode.RequestTimeout => "Request timed out. Please try again.",
                (HttpStatusCode)429 => "Too many requests. Please wait and try again.",
                HttpStatusCode.InternalServerError => "Server error. Please try again later.",
                HttpStatusCode.ServiceUnavailable => "Service temporarily unavailable.",
                _ => $"Request failed with status {(int)statusCode}."
            };

            _logger.Warning("ApiClient", $"API error {(int)statusCode} on {endpoint}: {message}");

            // Try to parse error response body for more detail
            try
            {
                var errorResult = JsonConvert.DeserializeObject<ApiResult<T>>(responseBody, JsonSettings);
                if (errorResult != null && !string.IsNullOrEmpty(errorResult.Message))
                {
                    return errorResult;
                }
            }
            catch { }

            return ApiResult<T>.ErrorResult(message);
        }

        /// <summary>
        /// Persists an API call record to the local database for diagnostics.
        /// </summary>
        private async Task LogApiCallAsync(string endpoint, string method, int statusCode,
            string? requestBody, string? responseBody, long durationMs, bool isSuccess)
        {
            try
            {
                var apiLog = new ApiLogModel
                {
                    Endpoint = endpoint,
                    Method = method,
                    StatusCode = statusCode,
                    RequestBody = (requestBody ?? string.Empty).Length > 2000
                        ? requestBody!.Substring(0, 2000) : requestBody ?? string.Empty,
                    ResponseBody = (responseBody ?? string.Empty).Length > 2000
                        ? responseBody!.Substring(0, 2000) : responseBody ?? string.Empty,
                    DurationMs = durationMs,
                    IsSuccess = isSuccess,
                    CreatedAt = DateTime.UtcNow
                };
                await _logRepository.InsertApiLogAsync(apiLog);
            }
            catch (Exception ex)
            {
                _logger.Warning("ApiClient", $"Failed to log API call: {ex.Message}");
            }
        }
    }
}
