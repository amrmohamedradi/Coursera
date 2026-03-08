using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Categories.Queries
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
}

