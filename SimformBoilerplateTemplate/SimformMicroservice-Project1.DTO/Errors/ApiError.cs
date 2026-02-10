namespace SimformMicroservice_Project1.DTO.Errors;

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}

public class ValidationError : ApiError
{
    public string Field { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
}