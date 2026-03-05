using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Commands.DeleteInstructor
{
    public record DeleteInstructorCommand(Guid Id) : IRequest<Unit>;
}
