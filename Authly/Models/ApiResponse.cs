namespace Authly.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        /// <summary>Chi tiết lỗi thật sự — chỉ hiện trong môi trường Development</summary>
        public string? Detail { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Message = message,
            Data = data,
        };

        public static ApiResponse<T> Fail(List<string> errors, string? message = null) => new()
        {
            Success = false,
            Message = message,
            Errors = errors,
        };

        public static ApiResponse<T> Fail(string error, string? message = null) =>
            Fail([error], message);
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse<object> Ok(string? message = null) => new()
        {
            Success = true,
            Message = message,
        };

        public new static ApiResponse<object> Fail(List<string> errors, string? message = null) => new()
        {
            Success = false,
            Message = message,
            Errors = errors,
        };

        public new static ApiResponse<object> Fail(string error, string? message = null) =>
            Fail([error], message);
    }
}
