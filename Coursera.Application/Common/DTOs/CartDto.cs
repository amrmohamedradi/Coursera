using Coursera.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Courses { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }

    }
}
