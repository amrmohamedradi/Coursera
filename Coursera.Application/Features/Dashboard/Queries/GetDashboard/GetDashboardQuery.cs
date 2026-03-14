using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Dashboard.Queries.GetDashboard
{
    public record GetDashboardQuery() : IRequest<DashboardDto>;
    
}
