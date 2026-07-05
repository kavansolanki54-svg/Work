namespace DallyWorkReoprt.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T? data, string message)
            => new ApiResponse<T> { Success = true, Data = data, Message = message };

        public static ApiResponse<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null)
            => new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                Data = default
            };
    }
}

