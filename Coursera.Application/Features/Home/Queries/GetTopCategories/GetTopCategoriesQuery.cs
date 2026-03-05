using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Home.Queries.GetTopCategories
{
    public record GetTopCategoriesQuery : IRequest<List<CategoryDto>>;
    
}
