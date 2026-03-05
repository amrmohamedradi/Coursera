using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Home.Queries.GetTopInstroctors
{
    public record GetTopinstructorsQuery  : IRequest<List<InstructorDto>>;
    
    
}
