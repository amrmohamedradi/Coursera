using FluentValidation;

namespace Coursera.Application.Features.Auth.ExternalLogin
{
    public class ExternalLoginValidator : AbstractValidator<ExternalLoginCommand>
    {
        private static readonly string[] SupportedProviders = { "google", "facebook" };

        public ExternalLoginValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required.")
                .Must(p => SupportedProviders.Contains(p.ToLowerInvariant()))
                .WithMessage("Provider must be 'google' or 'facebook'.");

            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("IdToken is required.");
        }
    }
}
