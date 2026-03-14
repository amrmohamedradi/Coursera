using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public class DashboardDto
    {
        public int CoursesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int InstructorsCount { get; set; }
        public decimal MonthlySales{ get; set; }

    }
}
