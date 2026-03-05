using Coursera.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Domain.Entities
{
    public class OrderItem :BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid CourseId { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid courseId, decimal price)
        {
            OrderId = orderId;
            CourseId = courseId;
            Price = price;
        }
    }
}
