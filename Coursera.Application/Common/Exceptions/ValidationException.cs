using FluentValidation.Results;

namespace Coursera.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown by <c>ValidationBehavior</c> when one or more FluentValidation
    /// rules fail. <see cref="Errors"/> carries every failing property and its
    /// associated error messages so the API can return a structured 400 body.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>Property-name → error-message array map.</summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// Build from a list of FluentValidation <see cref="ValidationFailure"/>s.
        /// Groups failures by property name.
        /// </summary>
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base("One or more validation failures have occurred.")
        {
            Errors = failures
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }

        /// <summary>
        /// Simple string constructor for manual throws that don't originate
        /// from a validator (preserves backward-compat with existing call sites).
        /// </summary>
        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>
            {
                [string.Empty] = new[] { message }
            };
        }
    }
}

