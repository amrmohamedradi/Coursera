using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Commands.CreateInstructor
{
    public class CreateInstructorValifator : AbstractValidator<CreateInstructorCommand>
    {
        public CreateInstructorValifator() 
        {

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Bio)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.ImgagePath)
                .NotEmpty();
            
        
        }
    }
}
