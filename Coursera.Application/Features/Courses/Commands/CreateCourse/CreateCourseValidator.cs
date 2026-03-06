using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000);
            RuleFor(c => c.Price)
                .GreaterThan(0);
            RuleFor(x => x.CategoryId)
                .NotEmpty();
            RuleFor(x => x.InstructorId)
                .NotEmpty();
            RuleFor(x => x.ImagePath)
                .NotEmpty();


        }
    }
}
