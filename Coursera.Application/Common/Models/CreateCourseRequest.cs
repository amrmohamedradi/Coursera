using Coursera.Domain.Entities;
using Coursera.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Models
{
    public class CreateCourseRequest
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public decimal Rating { get;  set; } = default!;
        public string ImagePath { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = default!;
        public Level Level { get; set; } = default!;
        public Guid CategoryId { get; set; } = default!;
        public Guid InstructorId { get; set; } = default!;
    }
}
