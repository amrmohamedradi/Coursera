using Coursera.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Commands.UpdateInstructor
{
    public record UpdateInstructorCommand(Guid Id,string Name, JobTitle JobTitle, string Bio, string ImgagePath) : IRequest<Unit>;
}
