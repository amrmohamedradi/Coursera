using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Queries
{
    public class GetCourseQueryValidator : AbstractValidator<GetCourseQuery>
    {
        public GetCourseQueryValidator()
        {
            RuleFor(c => c.PageNumber)
                .GreaterThan(0);
            RuleFor(c => c.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
