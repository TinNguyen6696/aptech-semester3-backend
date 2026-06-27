namespace TaLentShowcase.API.Models;

public class ApiResponse<T>
{
    public T? Data { get; set; }

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public IDictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "Success") => new()
    {
        Data = data,
        Success = true,
        Message = message
    };

    public static ApiResponse<T> Fail(
        string message,
        IDictionary<string, string[]>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}
