namespace PetBoarding_Domain.Emails;

public sealed class EmailResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public Exception? Exception { get; init; }

    public static EmailResult Success() => new() { IsSuccess = true };
    
    public static EmailResult Failure(string errorMessage, Exception? exception = null) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
        Exception = exception
    };
}