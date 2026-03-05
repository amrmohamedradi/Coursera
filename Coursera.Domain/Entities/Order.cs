using Coursera.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Domain.Entities
{
    public class Order:BaseEntity
    {
        public Guid UserId { get;private set; }
        public decimal Tax { get; private set; }
        public decimal FinalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public decimal TotalPrice { get; private set; }
        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();


        private Order() { }
        public Order(Guid userId,List<OrderItem> cartItems)
        {
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            foreach(var item in cartItems)
            {
                Items.Add(new OrderItem(Id, item.CourseId, item.Price));
            }
                TotalPrice = Items.Sum(i => i.Price);
                Tax = TotalPrice * 0.1m; 
                FinalPrice = TotalPrice + Tax;
        }
    }
}
