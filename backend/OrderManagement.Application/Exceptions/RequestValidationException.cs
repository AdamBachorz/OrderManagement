namespace OrderManagement.Application.Exceptions;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public RequestValidationException(string key, string message)
        : this(new Dictionary<string, string[]> { [key] = [message] })
    {
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
