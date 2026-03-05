using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Queries
{
    public record GetInstructorQueryById(Guid Id) : IRequest<InstructorDto>;
    
}
