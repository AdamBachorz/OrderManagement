namespace OrderManagement.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a domain rule is violated. 
    /// This exception is used to indicate that an operation cannot be completed due to a business rule violation or an invalid state in the domain model.
    /// </summary>
    public sealed class DomainException(string message) : Exception(message);
}
