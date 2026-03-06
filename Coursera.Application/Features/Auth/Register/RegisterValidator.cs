using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MinimumLength(100);
            RuleFor(x => x.LastNeme)
                .NotEmpty()
                .MinimumLength(100);
        }
    }
}
