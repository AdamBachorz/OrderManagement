using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Common
{
    internal static class Guard
    {
        /// <summary>
        /// null            -> ArgumentNullException,
        /// empty / white space   -> DomainException,
        /// too long        -> DomainException.
        /// </summary>
        public static string RequiredText(string? value, string paramName, int maxLength)
        {
            ArgumentNullException.ThrowIfNull(value, paramName);

            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{paramName} cannot be empty.");

            if (value.Length > maxLength)
                throw new DomainException($"{paramName} cannot be longer than {maxLength} characters.");

            return value;
        }
    }
}
