namespace sourceName.DTO;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static BaseResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new BaseResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static BaseResponse<T> Success(string message)
    {
        return new BaseResponse<T>
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static BaseResponse<T> Failure(string message, List<string>? errors = null)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? []
        };
    }

    public static BaseResponse<T> ValidationFailure(List<string> validationErrors)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = "Validation failed",
            Errors = validationErrors
        };
    }
}
