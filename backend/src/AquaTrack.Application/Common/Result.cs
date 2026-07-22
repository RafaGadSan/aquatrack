namespace AquaTrack.Application.Common;

/// <summary>
/// Wraps an expected business-flow outcome (e.g. "wrong password") that isn't exceptional and
/// shouldn't unwind the stack via an exception. Reserve exceptions for actual invariant violations.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(string error) => new(false, default, error);
}
