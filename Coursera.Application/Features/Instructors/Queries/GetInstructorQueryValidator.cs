using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Queries
{
    public class GetInstructorQueryValidator : AbstractValidator<GetInstructorQuery>
    {
        public GetInstructorQueryValidator()
        {
            RuleFor(c => c.PageNumber)
                 .GreaterThan(0);
            RuleFor(c => c.PageSize)
                .InclusiveBetween(1, 100);      
        }
    }
}
