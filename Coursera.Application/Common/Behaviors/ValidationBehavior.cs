using FluentValidation;
using MediatR;
using Coursera.Application.Common.Exceptions;

namespace Coursera.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that automatically runs every
    /// <see cref="IValidator{TRequest}"/> registered in DI before the handler
    /// executes.
    ///
    /// <para>
    /// When no validators exist for a request type the behavior is a no-op.
    /// When validators exist all of them are executed concurrently.
    /// If any rule fails a <see cref="ValidationException"/> carrying the full
    /// structured error map is thrown — which the global
    /// <c>ExceptionMiddleware</c> catches and converts to HTTP 400.
    /// </para>
    /// </summary>
    /// <typeparam name="TRequest">MediatR request type (Command or Query).</typeparam>
    /// <typeparam name="TResponse">Handler return type.</typeparam>
    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Fast-path: no validators registered for this request type.
            if (!_validators.Any())
                return await next();

            // Run all validators concurrently and collect every failure.
            var validationTasks = _validators
                .Select(v => v.ValidateAsync(
                    new ValidationContext<TRequest>(request),
                    cancellationToken));

            var validationResults = await Task.WhenAll(validationTasks);

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0)
                throw new Exceptions.ValidationException(failures);

            return await next();
        }
    }
}
