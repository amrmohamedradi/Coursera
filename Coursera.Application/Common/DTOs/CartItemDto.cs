using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public class CartItemDto
    {
        public Guid CourseId { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string ImagePath { get; set; }
    }
}
