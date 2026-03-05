using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, string ImagePath) : IRequest<Guid>;
    
}
